mergeInto(LibraryManager.library, {
	MindAR_storeCallbacks: function(positionArray, rotationArray, scaleArray, invokeTargetFoundCallback, invokeTargetLostCallback, setVisibilityCallback, invokeArReadyCallback, invokeArErrorCallback, setCameraProjectionMatrixCallback) {
		Module['MindAR'] = Module['MindAR'] || {};
		Module['MindAR'].positionArrayRef = Module['MindAR'].positionArrayRef || positionArray;
		Module['MindAR'].rotationArrayRef = Module['MindAR'].rotationArrayRef || rotationArray;
		Module['MindAR'].scaleArrayRef = Module['MindAR'].scaleArrayRef || scaleArray;
		
		Module['MindAR'].positionArray = Module['MindAR'].positionArray || new Float32Array(HEAP8.buffer, Module['MindAR'].positionArrayRef, 3 * _MindAR_getNumberMindARImageTargets());
		Module['MindAR'].rotationArray = Module['MindAR'].rotationArray || new Float32Array(HEAP8.buffer, Module['MindAR'].rotationArrayRef, 4 * _MindAR_getNumberMindARImageTargets());
		Module['MindAR'].scaleArray = Module['MindAR'].scaleArray || new Float32Array(HEAP8.buffer, Module['MindAR'].scaleArrayRef, _MindAR_getNumberMindARImageTargets());
		
		Module['MindAR'].invokeTargetFoundCallback = Module['MindAR'].invokeTargetFoundCallback || invokeTargetFoundCallback;
		Module['MindAR'].invokeTargetLostCallback = Module['MindAR'].invokeTargetLostCallback || invokeTargetLostCallback;
		Module['MindAR'].setVisibilityCallback = Module['MindAR'].setVisibilityCallback || setVisibilityCallback;
		Module['MindAR'].invokeArReadyCallback = Module['MindAR'].invokeArReadyCallback || invokeArReadyCallback;
		Module['MindAR'].invokeArErrorCallback = Module['MindAR'].invokeArErrorCallback || invokeArErrorCallback;
		Module['MindAR'].setCameraProjectionMatrixCallback = Module['MindAR'].setCameraProjectionMatrixCallback || setCameraProjectionMatrixCallback;
	},
	MindAR_getNumberMindARImageTargets: function() {
		return parseInt(document.getElementById('MindAR-scene').getAttribute('mindar-image').numberTrack);
	},
	MindAR_arReady: function() {
		document.getElementById('MindAR-scene').addEventListener('arReady', function(event) {
			Module.dynCall_v(Module['MindAR'].invokeArReadyCallback);
		});
	},
	MindAR_arError: function() {
		document.getElementById('MindAR-scene').addEventListener('arError', function(event) {
			Module.dynCall_v(Module['MindAR'].invokeArErrorCallback);
		});
	},
	MindAR_isRunning: function() {
		return !Module['MindAR'] || !Module['MindAR'].started ? false : Module['MindAR'].started ? true : false;
	}
});