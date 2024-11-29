mergeInto(LibraryManager.library, {
	DeviceCameraWebGL_start: function(callback, constraints, selector, canUpdateTexture) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_start(callback, constraints, selector, canUpdateTexture);
	},
	DeviceCameraWebGL_stop: function(callback, selector) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_stop(callback, selector);
	},
	DeviceCameraWebGL_isPlaying: function(selector) {
		return Module['DeviceCameraWebGL'].DeviceCameraWebGL_isPlaying(selector);
	},
	DeviceCameraWebGL_getDevices: function(callback, selector) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_getDevices(callback, selector);
	},
	DeviceCameraWebGL_createVideoElement: function(callback, id, css) {//"pointer-events: none; width: 1px; height: 1px"
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_createVideoElement(callback, id, css);
	},
	DeviceCameraWebGL_assignResizeEventToVideo: function(callback, selector) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_assignResizeEventToVideo(callback, selector);
	},
	DeviceCameraWebGL_assignPlayingEventToVideo: function(callback, selector, canUpdateTexture) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_assignPlayingEventToVideo(callback, selector, canUpdateTexture);
	},
	DeviceCameraWebGL_assignCSSToVideo: function(selector, css) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_assignCSSToVideo(selector, css);
	},
	DeviceCameraWebGL_updateTexture: function(selector, texturePtr) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_updateTexture(selector, texturePtr);
	},
	DeviceCameraWebGL_getCurrentFacingMode: function(selector) {
		return Module['DeviceCameraWebGL'].DeviceCameraWebGL_getCurrentFacingMode(selector);
	},
	DeviceCameraWebGL_isDesktop: function(selector) {
		return Module['DeviceCameraWebGL'].DeviceCameraWebGL_isDesktop(selector);
	},
	DeviceCameraWebGL_DestroyVideo: function(selector) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_DestroyVideo(selector);
	},
	DeviceCameraWebGL_waitForElement: function(callback, selector) {
		Module['DeviceCameraWebGL'].DeviceCameraWebGL_waitForElement(callback, selector);
	}
});