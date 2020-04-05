var LogReader = LogReader || {};

window.LogReader.app = (function() {

	this.LogReader.LogEntry = function(entry) {
		this.date = entry.date;
		this.type = entry.type;
		this.evt = entry.evt;
		this.do = entry.do;
		this.url = entry.url;
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
			switch (this.type) {
				case "Info":
					clone.children[0].classList.add("alert-info");
					break;
				case "Milestone":
					clone.children[0].classList.add("alert-primary");
					break;
				case "Warning":
					clone.children[0].classList.add("alert-warning");
					break;
				case "Error":
					clone.children[0].classList.add("alert-danger");
					break;
				case "Fatal":
					clone.children[0].classList.add("alert-danger");
					break;
				default:
					break;
			}
		}
		if(this.evt !== undefined){
			clone.querySelector(".logEntry__evt").textContent += this.evt;
		}
		if(this.do !== undefined){
			clone.querySelector(".logEntry__obj").textContent = this.do;
		}
		if(this.url !== undefined){
			clone.querySelector(".logEntry__obj").innerHTML = "<a href=\"" + this.url.href + "\" class=\"alert-link\" >" + this.url.text + "</a>";
		}
		if(this.innerText !== undefined){
			clone.querySelector(".logEntry__innerText").textContent = this.innerText;
		}
		if(this.innerHtml !== undefined){
			clone.querySelector(".logEntry__innerHtml").innerHTML = this.innerHtml;
		}
		if(this.ex !== undefined){
			clone.querySelector(".logEntry__ex").textContent = this.ex;
		}

        return clone;
    }
	

	var app = {
		logFile: 'log.txt',
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
				
				var log = { };
				if(logParts.length > 0){
					log.date = logParts[0];
				}
				if(logParts.length > 1){
					log.type = logParts[1].trim();
				}

				if(logParts.length > 2){
					for (let i = 2; i < logParts.length; i++) {
						var parts = logParts[i].match(/^(?<E>.+): (?<S>[\s\S^]*)/);// /^(EVT: |URL: |DO: |EX: |INNER_TEXT: |INNER_HTML: )([\s\S^]*)/
						var txt = parts.groups.S;

						switch (parts.groups.E) {
							case "EVT":
								log.evt = txt;
								break;
							case "DO":
								log.do = txt;
								break;
							case "URL":
								var pattern = /^http[s]?:\/\/.*?\/(?<L>[a-zA-Z-_]+).*$/;
								var m = txt.match(pattern);
								log.url = {}
								log.url.href = txt;
								log.url.text = m?.groups?.L;
								break;
							case "INNER_TEXT":
								log.innerText = txt;
								break;
							case "EX":
								log.ex = txt;
								break;
							case "INNER_HTML":
								log.innerHtml = txt;
								break;
							default:
								break;
						}
					}
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