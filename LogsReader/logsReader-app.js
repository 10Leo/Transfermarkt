(function() {

	this.LogEntry = function(entry) {
		this.date = entry.date;
		this.type = entry.type;
		this.msg = entry.msg;
		this.innerText = entry.innerText;
		this.innerHtml = entry.innerHtml;
		this.ex = entry.ex;
	}

    // Define our constructor
    this.Log = function() {
		this.logFile = GetLogFile('\log_20200328.txt');
    }

    Log.prototype.format = function() {
		var html = [];
		var node = document.createElement("tr");
		
		node.innerHTML = html.join('');
        return node;
    }
	
	function appendLog(parentNode, logEntry){
		var node = document.createElement("tr");

		var logDate = document.createElement("td");
		var logType = document.createElement("td");
		var logMsg = document.createElement("td");
		var logInnerText = document.createElement("td");
		var logInnerHtml = document.createElement("td");
		var logEx = document.createElement("td");

		if(logEntry.date !== undefined){
			logDate.innerHTML = logEntry.date;
		}
		if(logEntry.type !== undefined){
			logType.innerHTML = logEntry.type;
		}
		if(logEntry.msg !== undefined){
			logMsg.innerHTML = logEntry.msg;
		}
		if(logEntry.innerText !== undefined){
			logInnerText.innerHTML = logEntry.innerText;
		}
		if(logEntry.innerHtml !== undefined){
			logInnerHtml.innerHTML = logEntry.innerHtml;
		}
		if(logEntry.ex !== undefined){
			logEx.innerHTML = logEntry.ex;
		}

		node.appendChild(logDate);
		node.appendChild(logType);
		node.appendChild(logMsg);
		node.appendChild(logInnerText);
		node.appendChild(logInnerHtml);
		node.appendChild(logEx);

		parentNode.appendChild(node);
	}

	function GetLogFile(file){
		this.file = fetch(file)
			.then(response => response.text())
			.then((data) => {

				var parentNode = document.getElementById('tabLogs').querySelector("tbody");
				if (typeof(parentNode) === "undefined" && some_variable === null) {
					return;
				}

				var entryLogs = data.match(/(?<=\[\[)([^\[][^\[]|[^\]][^\]]*)(?=\s*\]\])/g);
				entryLogs.forEach(entryLog => {
					var logParts = entryLog.match(/(?<=\<\|\|)([^\<][^\|][^\|]|[^\|][^\|][^\>]*)(?=\s*\|\|\>)/g);

					var log = {};
					if(logParts.length > 0){
						log.date = logParts[0];
					}
					if(logParts.length > 1){
						log.type = logParts[1];
					}
					if(logParts.length > 2){
						log.msg = logParts[2];
					}
					if(logParts.length > 3){
						log.innerText = logParts[3];
					}
					if(logParts.length > 4){
						log.innerHtml = logParts[4];
					}
					if(logParts.length > 5){
						log.ex = logParts[5];
					}
					
					var logEntry = new LogEntry(log);
					appendLog(parentNode, logEntry);
				});
				
				return data;
			})
	}
}());

function init() {
	var d = document.getElementById('content');

	var t = new Log();

	var f = t.file;
	// for (var i = 0; i < todo.list.length; i++) {
	// 	var t = new Log(todo.list[i]);
	// 	d.appendChild(t.format());
	// }
}

$(document).ready(function() {
	init();
});
