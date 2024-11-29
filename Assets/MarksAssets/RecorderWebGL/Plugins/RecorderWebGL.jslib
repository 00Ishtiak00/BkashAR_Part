mergeInto(LibraryManager.library, {
	CreateMediaRecorder_RecorderWebGL: function(callback, options, microphone, ingameAudio, frameRate, target) {
		Module["RecorderWebGL"].setCurrentConfig(options, microphone, ingameAudio, frameRate);
		Module["RecorderWebGL"].createMediaRecorder(target).then(function(result) {
			callback !== 0 && Module.dynCall_vi(callback, 0);//success
		}).catch(function(error) {
			switch (error.name) {
				case "NotAllowedError": callback !== 0 && Module.dynCall_vi(callback, 1); break;
				case "MediaRecorderAPIUnsupportedError": callback !== 0 && Module.dynCall_vi(callback, 2); break;
			}

			console.error(error);
		});
	},
	CreateMediaRecorderNoFrameRate_RecorderWebGL: function(callback, options, microphone, ingameAudio, target) {
		Module["RecorderWebGL"].setCurrentConfig(options, microphone, ingameAudio, undefined);
		Module["RecorderWebGL"].createMediaRecorder(target).then(function(result) {
			callback !== 0 && Module.dynCall_vi(callback, 0);//success
		}).catch(function(error) {
			switch (error.name) {
				case "NotAllowedError": callback !== 0 && Module.dynCall_vi(callback, 1); break;
				case "MediaRecorderAPIUnsupportedError": callback !== 0 && Module.dynCall_vi(callback, 2); break;
			}

			console.error(error);
		});
	}, 
	StartNoTimeslice_RecorderWebGL: function(callback) {
        if (Module["RecorderWebGL"].mediaRecorder) {
			if (Module["RecorderWebGL"].mediaRecorder.state === "inactive") {
				if (!Module["RecorderWebGL"].mediaRecorder.stream.active) {
					Module["RecorderWebGL"].createMediaRecorder().then(function() {
						Module["RecorderWebGL"].mediaRecorder.start();
						Module["RecorderWebGL"].mediaRecorder.addEventListener('start', function(event) {
						callback !== 0 && Module.dynCall_v(callback);
					}, { once: true });
					}).catch(function(error) {
						console.error(error);
					});
				} else {
					Module["RecorderWebGL"].mediaRecorder.start();
					Module["RecorderWebGL"].mediaRecorder.addEventListener('start', function(event) {
						callback !== 0 && Module.dynCall_v(callback);
					}, { once: true });
				}
			}
		} else {
			if (Module["RecorderWebGL"].currentConfig) {
				Module["RecorderWebGL"].createMediaRecorder().then(function() {
					Module["RecorderWebGL"].mediaRecorder.start();
					Module["RecorderWebGL"].mediaRecorder.addEventListener('start', function(event) {
					callback !== 0 && Module.dynCall_v(callback);
				}, { once: true });
				}).catch(function(error) {
					console.error(error);
				});
			}
		}
	},
	Start_RecorderWebGL: function(timeslice, callback) {
		if (Module["RecorderWebGL"].mediaRecorder) {
			if (Module["RecorderWebGL"].mediaRecorder.state === "inactive") {
				if (!Module["RecorderWebGL"].mediaRecorder.stream.active) {
					Module["RecorderWebGL"].createMediaRecorder().then(function() {
						Module["RecorderWebGL"].mediaRecorder.start(timeslice);
						Module["RecorderWebGL"].mediaRecorder.addEventListener('start', function(event) {
							callback !== 0 && Module.dynCall_v(callback);
						}, { once: true });
					}).catch(function(error) {
						console.error(error);
					});
				} else {
					Module["RecorderWebGL"].mediaRecorder.start(timeslice);
					Module["RecorderWebGL"].mediaRecorder.addEventListener('start', function(event) {
						callback !== 0 && Module.dynCall_v(callback);
					}, { once: true });
				}
			}
		} else {
			if (Module["RecorderWebGL"].currentConfig) {
				Module["RecorderWebGL"].createMediaRecorder().then(function() {
					Module["RecorderWebGL"].mediaRecorder.start(timeslice);
					Module["RecorderWebGL"].mediaRecorder.addEventListener('start', function(event) {
						callback !== 0 && Module.dynCall_v(callback);
					}, { once: true });
				}).catch(function(error) {
					console.error(error);
				});
			}
		}
	},
	Stop_RecorderWebGL: function(callback, save, fileName) {
		if (Module["RecorderWebGL"].mediaRecorder) {
			if (Module["RecorderWebGL"].mediaRecorder.state !== "inactive") {
				Module["RecorderWebGL"].mediaRecorder.stop();
				Module["RecorderWebGL"].mediaRecorder.addEventListener('stop', function(event) {
					function runCallbacks() {
						if (Module["RecorderWebGL"].mediaRecorderBlob.metaDataAdded) {
							callback !== 0 && Module.dynCall_v(callback);
							save && Module["RecorderWebGL"].save(fileName);
							clearInterval(interval);
						}
					}
					const interval = setInterval(runCallbacks, 0);
				}, { once: true });
			} else {
				Module["RecorderWebGL"].mediaRecorder.onstop();
			}
		}
	},
	StopDataURL_RecorderWebGL: function(callback) {
		if (Module["RecorderWebGL"].mediaRecorder) {
			if (Module["RecorderWebGL"].mediaRecorder.state !== "inactive") {
				Module["RecorderWebGL"].mediaRecorder.stop();
				Module["RecorderWebGL"].mediaRecorder.addEventListener('stop', function(event) {
					function runCallbacks() {
						if (Module["RecorderWebGL"].mediaRecorderBlob.metaDataAdded) {
							Module["RecorderWebGL"].toDataURL(callback);
							clearInterval(interval);
						}
					}
					const interval = setInterval(runCallbacks, 0);
				}, { once: true });
			} else {
				Module["RecorderWebGL"].mediaRecorder.onstop();
			}
		}
	},
	ToDataURL_RecorderWebGL: function(callback) {
		if (!Module["RecorderWebGL"].mediaRecorderBlob) return;
		Module["RecorderWebGL"].toDataURL(callback);
	},
	StopObjectURL_RecorderWebGL: function(callback) {
		if (Module["RecorderWebGL"].mediaRecorder) {
			if (Module["RecorderWebGL"].mediaRecorder.state !== "inactive") {
				Module["RecorderWebGL"].mediaRecorder.stop();
				Module["RecorderWebGL"].mediaRecorder.addEventListener('stop', function(event) {
					function runCallbacks() {
						if (Module["RecorderWebGL"].mediaRecorderBlob.metaDataAdded) {
							Module["RecorderWebGL"].toObjectURL(callback);
							clearInterval(interval);
						}
					}
					const interval = setInterval(runCallbacks, 0);
				}, { once: true });
			} else {
				Module["RecorderWebGL"].mediaRecorder.onstop();
			}
		}
	},
	ToObjectURL_RecorderWebGL: function(callback) {
		if (!Module["RecorderWebGL"].mediaRecorderBlob) return;
		Module["RecorderWebGL"].toObjectURL(callback);
	},
	RevokeObjectURL_RecorderWebGL: function(url) {
		(window.URL ? URL : webkitURL).revokeObjectURL(UTF8ToString(url));
	},
	StopByteArray_RecorderWebGL: function(callback) {
		if (Module["RecorderWebGL"].mediaRecorder) {
			if (Module["RecorderWebGL"].mediaRecorder.state !== "inactive") {
				Module["RecorderWebGL"].mediaRecorder.stop();
				Module["RecorderWebGL"].mediaRecorder.addEventListener('stop', function(event) {
					function runCallbacks() {
						if (Module["RecorderWebGL"].mediaRecorderBlob.metaDataAdded) {
							Module["RecorderWebGL"].toByteArray(callback);
							clearInterval(interval);
						}
					}
					
					const interval = setInterval(runCallbacks, 0);
				}, { once: true });
			} else {
				Module["RecorderWebGL"].mediaRecorder.onstop();
			}
		}
	},
	ToByteArray_RecorderWebGL: function(callback) {
		if (!Module["RecorderWebGL"].mediaRecorderBlob) return;
		Module["RecorderWebGL"].toByteArray(callback);
	},
	Resume_RecorderWebGL: function(callback) {
		if (Module["RecorderWebGL"].mediaRecorder && Module["RecorderWebGL"].mediaRecorder.state !== "inactive") {
			Module["RecorderWebGL"].mediaRecorder.resume();
			Module["RecorderWebGL"].mediaRecorder.addEventListener("resume", (function(event) {
                callback !== 0 && Module.dynCall_v(callback);
            }), { once: true });
		}
	},
	Pause_RecorderWebGL: function(callback) {
		if (Module["RecorderWebGL"].mediaRecorder && Module["RecorderWebGL"].mediaRecorder.state !== "inactive") {
			Module["RecorderWebGL"].mediaRecorder.pause();
			Module["RecorderWebGL"].mediaRecorder.addEventListener("pause", (function(event) {
                callback !== 0 && Module.dynCall_v(callback);
            }), { once: true });
		}
	},
	Destroy_RecorderWebGL: function(callback) {
		if (Module["RecorderWebGL"].mediaRecorder) {
			if (Module["RecorderWebGL"].mediaRecorder.state !== "inactive") {
				Module["RecorderWebGL"].mediaRecorder.stop();
				Module["RecorderWebGL"].mediaRecorder.addEventListener('stop', function(event) {
					function runCallbacks() {
						if (Module["RecorderWebGL"].mediaRecorderBlob.metaDataAdded) {
							callback !== 0 && Module.dynCall_v(callback);
							Module["RecorderWebGL"].mediaRecorderBlob = null;
							Module["RecorderWebGL"].currentConfig = null;
							delete Module["RecorderWebGL"].mediaRecorder;
							clearInterval(interval);
						}
					}
					
					const interval = setInterval(runCallbacks, 0);
				}, { once: true });
			} else {
				Module["RecorderWebGL"].mediaRecorder.onstop();
				callback !== 0 && Module.dynCall_v(callback);
				Module["RecorderWebGL"].mediaRecorderBlob = null;
				Module["RecorderWebGL"].currentConfig = null;
				delete Module["RecorderWebGL"].mediaRecorder;
			}
		} else {
			Module["RecorderWebGL"].mediaRecorderBlob && (Module["RecorderWebGL"].mediaRecorderBlob = null);
			Module["RecorderWebGL"].currentConfig && (Module["RecorderWebGL"].currentConfig = null);
			callback !== 0 && Module.dynCall_v(callback);
		}
	},
	Discard_RecorderWebGL: function() {
		if (!Module["RecorderWebGL"].mediaRecorderBlob) return;
		Module["RecorderWebGL"].mediaRecorderBlob = null;
	},
	GetRecordingFileExtension_RecorderWebGL: function() {
		if (!Module["RecorderWebGL"].mediaRecorderBlob) return;
		var extension = Module["RecorderWebGL"].mediaRecorderBlob.type.split(";")[0].split("/")[1];
		if (!extension) extension = "failed";
		var bufferSize = lengthBytesUTF8(extension) + 1;
		var pointer = _malloc(bufferSize);
		stringToUTF8(extension, pointer, bufferSize);
		return pointer;
	},
	Save_RecorderWebGL: function(fileName) {
		Module["RecorderWebGL"].save(fileName);
	},
	GetCanvasWidth_RecorderWebGL: function() {
		return document.getElementsByTagName("canvas")[0].width;
	},
	GetCanvasHeight_RecorderWebGL: function() {
		return document.getElementsByTagName("canvas")[0].height;
	},
	IsTypeSupported_RecorderWebGL: function(type) {
		if (!window.MediaRecorder) return false;
		return MediaRecorder.isTypeSupported(UTF8ToString(type));
	},
	GetState_RecorderWebGL: function() {
		if (typeof Module["RecorderWebGL"].mediaRecorder !== 'undefined') {//it exists
			if (!Module["RecorderWebGL"].mediaRecorder) return 3;
			else return Module["RecorderWebGL"].mediaRecorder.state;
		} else {
			return -1;
		}
	},
	GetVideoBitsPerSecond_RecorderWebGL: function() {
		if (Module["RecorderWebGL"].mediaRecorder)
			return Module["RecorderWebGL"].mediaRecorder.videoBitsPerSecond;
		else
			return 0;
	},
	GetAudioBitsPerSecond_RecorderWebGL: function() {
		if (Module["RecorderWebGL"].mediaRecorder)
			return Module["RecorderWebGL"].mediaRecorder.audioBitsPerSecond;
		else
			return 0;
	},
	CanRecord_RecorderWebGL: function() {
		return (!window.MediaRecorder || !('onresume' in MediaRecorder.prototype)) ? false : true;
	},
	RequestFrame_RecorderWebGL: function() {
		if (!Module["RecorderWebGL"].mediaRecorder) return;
		Module["RecorderWebGL"].mediaRecorder.stream.getVideoTracks()[0].requestFrame();
	}
});