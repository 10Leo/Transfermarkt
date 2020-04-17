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

		if(this.date !== undefined && this.date.text !== ""){
			clone.querySelector(".logEntry__date").textContent = this.date.text;
		}
		if(this.type !== undefined && this.type.text !== ""){
			clone.querySelector(".logEntry__type").textContent = this.type.text;
			switch (this.type.text) {
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
		if(this.evt !== undefined && this.evt.text !== ""){
			clone.querySelector(".logEntry__evt").textContent += this.evt.text;
		}
		if(this.do !== undefined && this.do.text !== ""){
			clone.querySelector(".logEntry__obj").textContent = this.do.text;
		}
		if(this.url !== undefined && (this.url.text !== "" || this.url.href !== "")){
			clone.querySelector(".logEntry__obj").innerHTML = "<a href=\"" + this.url.href + "\" class=\"alert-link\" >" + this.url.text + "</a>";
		}
		if(this.innerText !== undefined && this.innerText.text !== ""){
			clone.querySelector(".logEntry__innerText").textContent = this.innerText.text;
		}
		if(this.innerHtml !== undefined && this.innerHtml.text !== ""){
			clone.querySelector(".logEntry__innerHtml").innerHTML = this.innerHtml.text;
		}
		if(this.ex !== undefined && this.ex.text !== ""){
			clone.querySelector(".logEntry__ex").textContent = this.ex.text;
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
				
				var log = {
					date: { text: "" },
					type: { text: "" },
					evt: { text: "" },
					do: { text: "" },
					url: { text: "", href: "" },
					ex: { text: "" },
					innerText: { text: "" },
					innerHtml: { text: "" }
				};

				if(logParts.length > 0){
					log.date.text = logParts[0];
				}
				if(logParts.length > 1){
					log.type.text = logParts[1].trim();
				}

				if(logParts.length > 2){
					for (let i = 2; i < logParts.length; i++) {
						var parts = logParts[i].match(/^(?<E>.+): (?<S>[\s\S^]*)/);// /^(EVT: |URL: |DO: |EX: |INNER_TEXT: |INNER_HTML: )([\s\S^]*)/
						var txt = parts.groups.S;
						
						var dict = {
							'EVT': log.evt,
							'DO': log.do,
							'URL': log.url,
							'EX': log.ex,
							'INNER_TEXT': log.innerText,
							'INNER_HTML': log.innerHtml
						};

						dict[parts.groups.E].text = txt

						if (parts.groups.E == 'URL') {
							var pattern = /^http[s]?:\/\/.*?\/(?<L>[a-zA-Z-_0-9]+).*$/;
							var m = txt.match(pattern);
							dict[parts.groups.E].text = m?.groups?.L;
							dict[parts.groups.E].href = txt;
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