mergeInto(LibraryManager.library, {
	JS_Sound_Init: function() {
		try {
			window.AudioContext = window.AudioContext || window.webkitAudioContext;
			WEBAudio.audioContext = new AudioContext;
			var tryToResumeAudioContext = function() {
				if (WEBAudio.audioContext.state === "suspended") WEBAudio.audioContext.resume();
				else Module.clearInterval(resumeInterval)
			};
			var resumeInterval = Module.setInterval(tryToResumeAudioContext, 400);
			WEBAudio.audioWebEnabled = 1;
			var _userEventCallback = function() {
				try {
					if (WEBAudio.audioContext.state !== "running" && WEBAudio.audioContext.state !== "closed") {
						WEBAudio.audioContext.resume()
					}
					jsAudioPlayBlockedAudios();
					var audioCacheSize = 20;
					while (WEBAudio.audioCache.length < audioCacheSize) {
						var audio = new Audio;
						audio.autoplay = false;
						WEBAudio.audioCache.push(audio)
					}
				} catch (e) {}
			};
			window.addEventListener("mousedown", _userEventCallback);
			window.addEventListener("touchstart", _userEventCallback);
			Module.deinitializers.push(function() {
				window.removeEventListener("mousedown", _userEventCallback);
				window.removeEventListener("touchstart", _userEventCallback)
			})
		} catch (e) {
			alert("Web Audio API is not supported in this browser")
		}
	}
});