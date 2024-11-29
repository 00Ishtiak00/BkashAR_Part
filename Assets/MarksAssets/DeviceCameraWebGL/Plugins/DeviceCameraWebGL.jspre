Module['DeviceCameraWebGL'] = Module['DeviceCameraWebGL'] || {};
Module['DeviceCameraWebGL'].resizeEvent = null;
Module['DeviceCameraWebGL'].playingEvent = null;
Module['DeviceCameraWebGL'].canUpdateTextureRef = null;
Module['DeviceCameraWebGL'].canUpdateTextureArr = null;
Module['DeviceCameraWebGL'].DeviceCameraWebGL_start = function(callback, constraints, selector, canUpdateTexture) {
	if (constraints === 0) return;
	
	const video = document.querySelector(UTF8ToString(selector));
	if (!video) return;
		
	if (Module['DeviceCameraWebGL'].canUpdateTextureRef && !(!!(video.currentTime > 0 && !video.paused && !video.ended && video.readyState > 2))) return;
	const getUserMedia = navigator.mediaDevices.getUserMedia.bind(navigator.mediaDevices) || navigator.getUserMedia.bind(navigator) || navigator.webkitGetUserMedia.bind(navigator) || navigator.mozGetUserMedia.bind(navigator) || navigator.msGetUserMedia.bind(navigator);
		
	Module['DeviceCameraWebGL'].canUpdateTextureRef = Module['DeviceCameraWebGL'].canUpdateTextureRef || canUpdateTexture;
	Module['DeviceCameraWebGL'].canUpdateTextureArr = Module['DeviceCameraWebGL'].canUpdateTextureArr || new Uint8Array(HEAP8.buffer, canUpdateTexture, 1);
	Module['DeviceCameraWebGL'].canUpdateTextureArr[0] = 0;
		
	constraints = JSON.parse(UTF8ToString(constraints));
	
	video.addEventListener('pause', function() {
		if ('srcObject' in video) {
			if (video.srcObject) {
				video.srcObject.getTracks()[0].stop();
				video.srcObject = null;
			}
		} else {
			if (video.src) {
				video.src.getTracks()[0].stop();
				video.src = null;
			}
		}
		
		getUserMedia(constraints).then(function(stream) {
			if ('srcObject' in video) {
				try {
					video.srcObject = stream
				} catch (err) {
					if (err.name !== 'TypeError') {
						throw err;
					}
					video.src = (window.URL ? URL : webkitURL).createObjectURL(stream);
				}
			} else {
				video.src = (window.URL ? URL : webkitURL).createObjectURL(stream);
			}
			video.play();
			video.addEventListener('playing', function() {
				callback !== 0 && Module.dynCall_vi(callback, 0);
				if (Module['DeviceCameraWebGL'].canUpdateTextureArr.byteLength === 0)//buffer resized, need to assign array again
					Module['DeviceCameraWebGL'].canUpdateTextureArr = new Uint8Array(HEAP8.buffer, Module['DeviceCameraWebGL'].canUpdateTextureRef, 1);
				Module['DeviceCameraWebGL'].canUpdateTextureArr[0] = 1;
			}, {once: true});
		}).catch(function(error) {
			console.error(error);
			if (callback !== 0) {
				switch (error.name) {
					case 'AbortError': Module.dynCall_vi(callback, 1); break;
					case 'NotAllowedError': Module.dynCall_vi(callback, 2); break;
					case 'NotFoundError': Module.dynCall_vi(callback, 3); break;
					case 'NotReadableError': Module.dynCall_vi(callback, 4); break;
					case 'OverconstrainedError': Module.dynCall_vi(callback, 5); break;
					case 'SecurityError': Module.dynCall_vi(callback, 6); break;
					case 'TypeError': Module.dynCall_vi(callback, 7); break;
				}
			}
		});
	}, {once: true});
		
	if (!!(video.currentTime > 0 && !video.paused && !video.ended && video.readyState > 2))//video playing
		video.pause();
	else
		video.dispatchEvent(new Event('pause'));
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_stop = function(callback, selector) {
	selector = UTF8ToString(selector);
    const video = document.querySelector(selector);
    if (!video) return;
    if (!!!(video.currentTime > 0 && !video.paused && !video.ended && video.readyState > 2)) return;
    video.pause();
    video.addEventListener('pause', (function() {
		if ('srcObject' in video) {
			if (video.srcObject) video.srcObject.getTracks()[0].stop();
			video.srcObject = null
		} else if (video.src) {
			video.src.getTracks()[0].stop();
			video.src = null
		}
        callback !== 0 && Module.dynCall_v(callback)
    }), {
        once: true
    });
        
    if (Module['DeviceCameraWebGL'].canUpdateTextureArr.byteLength === 0) Module['DeviceCameraWebGL'].canUpdateTextureArr = new Uint8Array(HEAP8.buffer, Module['DeviceCameraWebGL'].canUpdateTextureRef, 1);
    Module['DeviceCameraWebGL'].canUpdateTextureArr[0] = 0;
    Module['DeviceCameraWebGL'].canUpdateTextureRef = null
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_isPlaying = function(selector) {
	selector = UTF8ToString(selector);
    const video = document.querySelector(selector);
    if (!video) return false;
	return !!(video.currentTime > 0 && !video.paused && !video.ended && video.readyState > 2);
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_getDevices = function(callback, selector) {
	selector = UTF8ToString(selector);
	const video = document.querySelector(selector);
	if (!video) return;
	navigator.mediaDevices.enumerateDevices()
	.then(function(devices) {
		var mediaDevices = {
			devices: []
		};
		devices.forEach(function(device) {
			mediaDevices.devices.push(device);
		});
			
		mediaDevices = JSON.stringify(mediaDevices);
		var bufferSize = lengthBytesUTF8(mediaDevices) + 1;
		var pointer = _malloc(bufferSize);
		stringToUTF8(mediaDevices, pointer, bufferSize);
			
		Module.dynCall_vi(callback, pointer);
	})
	.catch(function(err) {
		console.error(err.name + ': ' + err.message);
	});
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_createVideoElement = function(callback, id, css) {//'pointer-events: none; width: 1px; height: 1px'
	id = UTF8ToString(id);
	css = UTF8ToString(css);
	if (document.getElementById(id)) return;//if an element with the same id already exists, exit.
	const video = document.createElement('video');
	video.setAttribute('id', id);
	video.setAttribute('playsinline', true);
	video.setAttribute('style', css);
	video.muted = true;//necessary for MS Edge
		
	if (callback !== 0) {
		video.removeEventListener('resize', Module['DeviceCameraWebGL'].resizeEvent);
		Module['DeviceCameraWebGL'].resizeEvent = function(e) {
			Module.dynCall_vii(callback, e.target.videoWidth, e.target.videoHeight);
		}
		video.addEventListener('resize', Module['DeviceCameraWebGL'].resizeEvent);
	}
		
	video.removeEventListener('playing', Module['DeviceCameraWebGL'].playingEvent);
		
	Module['DeviceCameraWebGL'].playingEvent = function() {
		if (Module['DeviceCameraWebGL'].canUpdateTextureArr.byteLength === 0)//buffer resized, need to assign array again
			Module['DeviceCameraWebGL'].canUpdateTextureArr = new Uint8Array(HEAP8.buffer, Module['DeviceCameraWebGL'].canUpdateTextureRef, 1);
				
		Module['DeviceCameraWebGL'].canUpdateTextureArr[0] = 1;
	}
		
		
	video.addEventListener('playing', Module['DeviceCameraWebGL'].playingEvent);
		
		
	document.querySelector('body').appendChild(video);
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_assignResizeEventToVideo = function(callback, selector) {
	selector = UTF8ToString(selector);
    const video = document.querySelector(selector);
    if (!video) return;
    if (callback !== 0) {
        window.removeEventListener('resize', Module['DeviceCameraWebGL'].windowResizeEvent);
        Module['DeviceCameraWebGL'].windowResizeEvent = function () {
            if (window.innerHeight > window.innerWidth) {
                if (Module['DeviceCameraWebGL'].firstResize !== 'portrait' && Module['DeviceCameraWebGL'].firstResize !== 'landscape') {
                    video.videoWidthbk = video.videoWidth;
                    video.videoHeightbk = video.videoHeight;
                    Module['DeviceCameraWebGL'].firstResize = 'portrait';
                } else {
                    if (Module['DeviceCameraWebGL'].firstResize == 'portrait') {
                        video.videoWidthbk = video.videoWidth;
                        video.videoHeightbk = video.videoHeight;
                    } else {
                        video.videoWidthbk = video.videoHeight;
                        video.videoHeightbk = video.videoWidth;
                    }
                        
                }
                    
                Module.dynCall_vii(callback, video.videoWidthbk, video.videoHeightbk)
            } else {
                if (Module['DeviceCameraWebGL'].firstResize !== 'portrait' && Module['DeviceCameraWebGL'].firstResize !== 'landscape') {
                    video.videoWidthbk = video.videoHeight;
                    video.videoHeightbk = video.videoWidth;
                    Module['DeviceCameraWebGL'].firstResize = 'landscape';
                } else {
                    if (Module['DeviceCameraWebGL'].firstResize == 'landscape') {
                        video.videoWidthbk = video.videoHeight;
                        video.videoHeightbk = video.videoWidth;
                    } else {
                        video.videoWidthbk = video.videoWidth;
                        video.videoHeightbk = video.videoHeight;
                    }
                }

                Module.dynCall_vii(callback, video.videoHeightbk, video.videoWidthbk)
            }

        };
        window.addEventListener('resize', Module['DeviceCameraWebGL'].windowResizeEvent)
        window.dispatchEvent(new Event('orientationchange'));
		window.dispatchEvent(new Event('resize'));//for iOS
    }
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_assignPlayingEventToVideo = function(callback, selector, canUpdateTexture) {
	selector = UTF8ToString(selector);
	const video = document.querySelector(selector);
	if (!video) return;
		
	video.removeEventListener('playing', Module['DeviceCameraWebGL'].playingEvent);
		
	Module['DeviceCameraWebGL'].canUpdateTextureRef = Module['DeviceCameraWebGL'].canUpdateTextureRef || canUpdateTexture;
	Module['DeviceCameraWebGL'].canUpdateTextureArr = Module['DeviceCameraWebGL'].canUpdateTextureArr || new Uint8Array(HEAP8.buffer, canUpdateTexture, 1);
	Module['DeviceCameraWebGL'].canUpdateTextureArr[0] = 1;
	callback !== 0 && Module.dynCall_vi(callback, 0);
		
	Module['DeviceCameraWebGL'].playingEvent = function() {
		callback !== 0 && Module.dynCall_vi(callback, 0);
		if (Module['DeviceCameraWebGL'].canUpdateTextureArr.byteLength === 0)//buffer resized, need to assign array again
			Module['DeviceCameraWebGL'].canUpdateTextureArr = new Uint8Array(HEAP8.buffer, Module['DeviceCameraWebGL'].canUpdateTextureRef, 1);
				
		Module['DeviceCameraWebGL'].canUpdateTextureArr[0] = 1;
	}
		
		
	video.addEventListener('playing', Module['DeviceCameraWebGL'].playingEvent);
		
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_assignCSSToVideo = function(selector, css) {
	selector = UTF8ToString(selector);
	css = UTF8ToString(css);
	const video = document.querySelector(selector);
	if (!video) return;
	video.setAttribute('style', css);
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_updateTexture = function(selector, texturePtr) {
	selector = UTF8ToString(selector);
    const video = document.querySelector(selector);
    if (!video) return false;
    if (!(video.videoWidth > 0 && video.videoHeight > 0)) return false;

    if (video.previousUploadedWidth != video.videoWidth || video.previousUploadedHeight != video.videoHeight) {
        GLctx.deleteTexture(GL.textures[texturePtr]);
        GL.textures[texturePtr] = GLctx.createTexture();
        GL.textures[texturePtr].name = texturePtr;
        var prevTex = GLctx.getParameter(GLctx.TEXTURE_BINDING_2D);
        GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texturePtr]);
        GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, true);
        GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_S, GLctx.CLAMP_TO_EDGE);
        GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_T, GLctx.CLAMP_TO_EDGE);
        GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_MIN_FILTER, GLctx.LINEAR);
        GLctx.texImage2D(GLctx.TEXTURE_2D, 0, GLctx.RGBA, GLctx.RGBA, GLctx.UNSIGNED_BYTE, video);
        GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, false);
        GLctx.bindTexture(GLctx.TEXTURE_2D, prevTex);
        video.previousUploadedWidth = video.videoWidth;
        video.previousUploadedHeight = video.videoHeight
    } else {
        GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, true);
        var prevTex = GLctx.getParameter(GLctx.TEXTURE_BINDING_2D);
        GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texturePtr]);
        GLctx.texImage2D(GLctx.TEXTURE_2D, 0, GLctx.RGBA, GLctx.RGBA, GLctx.UNSIGNED_BYTE, video);
        GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, false);
        GLctx.bindTexture(GLctx.TEXTURE_2D, prevTex)
    }
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_getCurrentFacingMode = function(selector) {
	selector = UTF8ToString(selector);
	const video = document.querySelector(selector);
	var facingMode;
	if (!video) facingMode = 'none';
	else {
		if ('srcObject' in video) {
			if (video.srcObject) facingMode = video.srcObject.getTracks()[0].getSettings().facingMode || 'user';//on Desktop the facingMode returns undefined
			else facingMode = 'none';
		} else if (video.src) {
			facingMode = video.src.getTracks()[0].getSettings().facingMode || 'user';
		} else {
			facingMode = 'none';
		}
	}
	var bufferSize = lengthBytesUTF8(facingMode) + 1;
	var pointer = _malloc(bufferSize);
	stringToUTF8(facingMode, pointer, bufferSize);
	return pointer;
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_isDesktop = function(selector) {
	selector = UTF8ToString(selector);
	const video = document.querySelector(selector);
	if (!video) return false;
	if ('srcObject' in video) {
		if (video.srcObject) {
			if (video.srcObject.getTracks()[0].getSettings().facingMode) return false;
			else return true;
		}
		else return false;
	} else if (video.src) {
		if (video.src.getTracks()[0].getSettings().facingMode) return false;
		else return true;
	} else {
		return false;
	}
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_DestroyVideo = function(selector) {
	selector = UTF8ToString(selector);
	var video = document.querySelector(selector);
	if (!video) return;
	if ('srcObject' in video) {
		if (video.srcObject) video.srcObject.getTracks()[0].stop();
		video.srcObject = null;
	} else if (video.src) {
		video.src.getTracks()[0].stop();
		video.src = null;
	}
		
	window.removeEventListener('resize', Module['DeviceCameraWebGL'].windowResizeEvent);
	video.removeEventListener('resize', Module['DeviceCameraWebGL'].resizeEvent);
	video.removeEventListener('playing', Module['DeviceCameraWebGL'].playingEvent);
		
	if (Module['DeviceCameraWebGL'].canUpdateTextureArr) {
		if (Module['DeviceCameraWebGL'].canUpdateTextureArr.byteLength === 0) Module['DeviceCameraWebGL'].canUpdateTextureArr = new Uint8Array(HEAP8.buffer, Module['DeviceCameraWebGL'].canUpdateTextureRef, 1);
		Module['DeviceCameraWebGL'].canUpdateTextureArr[0] = 0;
	}
		
	GLctx.bindTexture(GLctx.TEXTURE_2D, null);
    video.remove();
	video = Module['DeviceCameraWebGL'].playingEvent = Module['DeviceCameraWebGL'].resizeEvent = Module['DeviceCameraWebGL'].canUpdateTextureRef = Module['DeviceCameraWebGL'].canUpdateTextureArr = null;
};
Module['DeviceCameraWebGL'].DeviceCameraWebGL_waitForElement = function(callback, selector) {
	selector = UTF8ToString(selector);
		
	if (document.querySelector(selector)) {
		Module.dynCall_v(callback);
		return;
	}
		
	var observer = new MutationObserver(function (mutations) {
        if (document.querySelector(selector)) {
            Module.dynCall_v(callback);
			observer.disconnect();
			return;
        }
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true,
		attributes: false,
		characterData: false
    });
}