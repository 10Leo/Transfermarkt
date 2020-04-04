var LogReader = LogReader || {};

window.LogReader.app = (function() {

	this.LogReader.LogEntry = function(entry) {
		this.date = entry.date;
		this.type = entry.type;
		this.msg = entry.msg;
		this.innerText = entry.innerText;
		this.innerHtml = entry.innerHtml;
		this.ex = entry.ex;
	}

	this.LogReader.LogEntry.prototype.CreateUI = function() {
		var temp = document.getElementById("logEntry-template");
        var clone = temp.content.cloneNode(true);

		if(this.date !== undefined){
			clone.querySelector(".logEntry__date").textContent = this.date;
		}
		if(this.type !== undefined){
			clone.querySelector(".logEntry__type").textContent = this.type;
		}
		if(this.msg !== undefined){
			clone.querySelector(".logEntry__msg").textContent = this.msg;
		}
		if(this.innerText !== undefined){
			clone.querySelector(".logEntry__innerText").textContent = this.innerText;
		}
		if(this.innerHtml !== undefined){
			clone.querySelector(".logEntry__innerHtml").innerHtml = this.innerHtml;
		}
		if(this.ex !== undefined){
			clone.querySelector(".logEntry__ex").textContent = this.ex;
		}

        return clone;
    }
	

	var app = {
		logFile: 'log_20200328.txt',
        logs: [],

        initialize: function () {
            this.bindEvents();
        },

        bindEvents: function () {
            if (window.location.pathname.indexOf('/logsReader.html') !== -1) {
                window.addEventListener('load', this.onDeviceReady.bind(this), false);
            }
        },

        onDeviceReady: function () {
            this.getLogs()
                .then(response => response.text())
                .then(app.parseLogs)
                .then(app.renderData);
		},
		
        getLogs: function () {
            return fetch(this.logFile);
		},
		
        parseLogs: function (data) {
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
				
				app.logs.push(
					new LogReader.LogEntry(log)
				);
			});
			
			return data;
		},
		
        renderData: function () {
            var basePane = document.getElementById('logs-list');
            app.logs.forEach(n => {
                var thisLogs = n.CreateUI();
                basePane.appendChild(thisLogs);
            });
        }
	};
	
    return app;
}());

LogReader.app.initialize();