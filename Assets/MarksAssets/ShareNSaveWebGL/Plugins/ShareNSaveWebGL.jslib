mergeInto(LibraryManager.library, {	
	ShareNSaveWebGL_Share: function(shareCallback, file, fileSize, mimeType, fileName, url, title, text) {
		const status = {Success: 0, NotAllowedError: 1, TypeError: 2, AbortError: 3, DataError: 4, InvalidStateError: 5, CantShareError: 6, NotABlobError: 7, WebShareUnsupportedError: 8, WebShare2UnsupportedError: 9};
	
		if ( (file !== 0 || fileSize !== 0) && (!navigator.canShare || !navigator.canShare({files: [new File(["dummy"], "dummy.txt", {type: "text/plain"})]})) ) {//trying to share a file and navigator.canShare is not supported
			Module.dynCall_vi(shareCallback, status.WebShare2UnsupportedError);
			return;
		} else if ( (url !== 0 || text !== 0 || title !== 0) && !navigator.share) {//trying to share text and navigator.share is not supported
			Module.dynCall_vi(shareCallback, status.WebShareUnsupportedError);
			return;
		} else if ( (file === 0 || fileSize === 0) && url === 0 && text === 0 && title === 0) //no file or text, nothing to share
			return;
			
		const shareData = {};
		
		fileName = fileName !== 0 ? UTF8ToString(fileName) : "file";
		mimeType = UTF8ToString(mimeType);
		if (fileName.indexOf(".") === -1) fileName = fileName + "." + mimeType.split(";")[0].split("/")[1];
		
		if (file !== 0 && fileSize !== 0) shareData.files = [new File([HEAPU8.subarray(file, file + fileSize)], fileName, {type: mimeType})];
		if (url !== 0) shareData.url = UTF8ToString(url);
		if (text !== 0) shareData.text = UTF8ToString(text);
		if (title !== 0) shareData.title = UTF8ToString(title);
		
		if (navigator.canShare && !navigator.canShare(shareData)) {//data can't be shared
			Module.dynCall_vi(shareCallback, status.CantShareError);
			return;
		}
		
		document.documentElement.addEventListener('pointerup', function () {
			navigator.share(shareData)
				.then (function()    {Module.dynCall_vi(shareCallback, status.Success);})
				.catch(function(err) {Module.dynCall_vi(shareCallback, status[err.name]);});
		}, { once: true });
		
	},
	ShareNSaveWebGL_ShareBlob: function(shareCallback, blob, fileName, url, title, text) {
		//if the value is null from C#, it's 0 here.
		const status = {Success: 0, NotAllowedError: 1, TypeError: 2, AbortError: 3, DataError: 4, InvalidStateError: 5, CantShareError: 6, NotABlobError: 7, WebShareUnsupportedError: 8, WebShare2UnsupportedError: 9};
		var isCallingFromJS = false;
		
		if (typeof fileName === 'string') isCallingFromJS = true;
		
		if (!(blob instanceof Blob)) {//the blob is actually the property path that retrieves the blob
			if (blob) {//if trying to share a blob. If it's 0, no blob is trying to be shared.
				blob = !isCallingFromJS ? UTF8ToString(blob) : blob;
				if (typeof blob !== 'string') {
					!isCallingFromJS ? Module.dynCall_vi(shareCallback, status.NotABlobError) : shareCallback(status.NotABlobError);//not instance of blob and not a string, this is something else that is not a blob.
					return;
				}
				function getBlob(propertyPath) {
					propertyPath = propertyPath;
					var object = (function(root) {
						return root === "Module" ? Module : root === "window" ? window : root === "document" ? document : undefined;
					})(propertyPath.split('.')[0]);
					if (!object) return undefined;
					propertyPath = propertyPath.split('.').slice(1).join('.');
					var resolvePath = function resolvePath(object, path, defaultValue) {
					  return path.split('.').reduce(function (o, p) {
						return o ? o[p] : defaultValue;
					  }, object);
					};
					return resolvePath(object, propertyPath);
				}
				blob = getBlob(blob);
				if (!(blob instanceof Blob)) {
					!isCallingFromJS ? Module.dynCall_vi(shareCallback, status.NotABlobError) : shareCallback(status.NotABlobError);//the path doesn't result in a blob
					return;
				}
			}
		}
		
		if ( blob && (!navigator.canShare || !navigator.canShare({files: [new File(["dummy"], "dummy.txt", {type: "text/plain"})]}))) {//trying to share a file and navigator.canShare is not supported
			!isCallingFromJS ? Module.dynCall_vi(shareCallback, status.WebShare2UnsupportedError): shareCallback(status.WebShare2UnsupportedError)
			return;
		} else if ( (url || text || title) && !navigator.share) {//trying to share text and navigator.share is not supported
			!isCallingFromJS ? Module.dynCall_vi(shareCallback, status.WebShareUnsupportedError) : shareCallback(status.WebShareUnsupportedError);
			return;
		} else if (!blob && !url && !text && !title) //no file or text, nothing to share
			return;
			
			
		const shareData = {};
		
		if (!isCallingFromJS) {
			fileName = fileName !== 0 ? UTF8ToString(fileName) : "file";
		} else {
			if (fileName === "")
				fileName = "file";
		}
			
		if (blob) {
			if (fileName.indexOf(".") === -1) fileName = fileName + "." + blob.type.split(";")[0].split("/")[1];
			shareData.files = [new File([blob], fileName, {type: blob.type.split(";")[0]})];//ignore everything after ; if present, because it just has codec information that makes the sharing fail to work for some reason.
		}
		if (url) shareData.url = !isCallingFromJS ? UTF8ToString(url) : url;
		if (text) shareData.text = !isCallingFromJS ? UTF8ToString(text) : text;
		if (title) shareData.title = !isCallingFromJS ? UTF8ToString(title) : title;
		
		if (navigator.canShare && !navigator.canShare(shareData)) {//data can't be shared
			!isCallingFromJS ? Module.dynCall_vi(shareCallback, status.CantShareError) : shareCallback(status.CantShareError);
			return;
		}
		
		document.documentElement.addEventListener('pointerup', function () {
			navigator.share(shareData)
				.then (function()    {!isCallingFromJS ? Module.dynCall_vi(shareCallback, status.Success) : shareCallback(status.Success);})
				.catch(function(err) {!isCallingFromJS ? Module.dynCall_vi(shareCallback, status[err.name]) : shareCallback(status[err.name])});
		}, { once: true });
		
	},
	ShareNSaveWebGL_Save: function(file, fileSize, mimeType, fileName) {
		if (fileSize === 0) return;
		
		fileName = fileName !== 0 ? UTF8ToString(fileName) : "file";
		mimeType = UTF8ToString(mimeType);
		if (fileName.indexOf(".") === -1) fileName = fileName + "." + mimeType.split(";")[0].split("/")[1];
		
        file = new Blob([HEAPU8.subarray(file, file + fileSize)], {type: mimeType});
        if (window.navigator.msSaveOrOpenBlob != null) {
            window.navigator.msSaveBlob(file, fileName)
        } else {
            var link = document.createElement("a");
            link.download = fileName;
            link.style.display = "none";
            if (window.webkitURL != null) {
                link.href = window.webkitURL.createObjectURL(file)
            } else {
                link.href = window.URL.createObjectURL(file)
            }

            link.click();
			
			//setTimeout necessary for Safari
			setTimeout(function() {
			  window.URL.revokeObjectURL(link.href);
			}, 1000);
        }
	},
	ShareNSaveWebGL_SaveBlob: function(blob, fileName) {
		var isCallingFromJS = false;
		if (typeof fileName === 'string') isCallingFromJS = true;
		
		if (!(blob instanceof Blob)) {//the blob is actually the property path that retrieves the blob
			if (blob) {//if trying to share a blob. If it's 0, no blob is trying to be shared.
				blob = !isCallingFromJS ? UTF8ToString(blob) : blob;
				if (typeof blob !== 'string') return;//not instance of blob and not a string, this is something else that is not a blob.
				function getBlob(propertyPath) {
					propertyPath = propertyPath;
					var object = (function(root) {
						return root === "Module" ? Module : root === "window" ? window : root === "document" ? document : undefined;
					})(propertyPath.split('.')[0]);
					if (!object) return undefined;
					propertyPath = propertyPath.split('.').slice(1).join('.');
					var resolvePath = function resolvePath(object, path, defaultValue) {
					  return path.split('.').reduce(function (o, p) {
						return o ? o[p] : defaultValue;
					  }, object);
					};
					return resolvePath(object, propertyPath);
				}
				blob = getBlob(blob);
				if (!(blob instanceof Blob)) return;
			} else
				return;
		}
		
		if (!isCallingFromJS) {
			fileName = fileName !== 0 ? UTF8ToString(fileName) : "file";
		} else {
			if (fileName === "")
				fileName = "file";
		}
		
		if (fileName.indexOf(".") === -1) fileName = fileName + "." + blob.type.split(";")[0].split("/")[1];
	
        if (window.navigator.msSaveOrOpenBlob != null) {
            window.navigator.msSaveBlob(blob, fileName)
        } else {
            var link = document.createElement("a");
            link.download = fileName;
            link.style.display = "none";
            if (window.webkitURL != null) {
                link.href = window.webkitURL.createObjectURL(blob)
            } else {
                link.href = window.URL.createObjectURL(blob)
            }

            link.click();
			
			//setTimeout necessary for Safari
			setTimeout(function() {
			  window.URL.revokeObjectURL(link.href);
			}, 1000);
        }
	},
	ShareNSaveWebGL_CanShare: function(file, fileSize, mimeType, fileName, url, title, text) {
		const status = {Success: 0, CantShareError: 6, WebShareUnsupportedError: 8, WebShare2UnsupportedError: 9};
		if (file !== 0 || fileSize !== 0) {
			if (!navigator.canShare || !navigator.canShare({files: [new File(["dummy"], "dummy.txt", {type: "text/plain"})]}))//trying to share a file and navigator.canShare is not supported
				return status.WebShare2UnsupportedError;
			else {
				const shareData = {};
				
				fileName = fileName !== 0 ? UTF8ToString(fileName) : "file";
				mimeType = UTF8ToString(mimeType);
				if (fileName.indexOf(".") === -1) fileName = fileName + "." + mimeType.split(";")[0].split("/")[1];
		
				if (file !== 0 && fileSize !== 0) shareData.files = [new File([HEAPU8.subarray(file, file + fileSize)], fileName, {type: mimeType})];
				if (url !== 0) shareData.url = UTF8ToString(url);
				if (text !== 0) shareData.text = UTF8ToString(text);
				if (title !== 0) shareData.title = UTF8ToString(title);
				
				if (navigator.canShare(shareData)) return status.Success;
				else return status.CantShareError;
			}
		} else {
			if (url !== 0 || text !== 0 || title !== 0) {//if trying to share text only
				if (navigator.share) return status.Success;//they can share
				else return status.WebShareUnsupportedError;//they can't share
			} else {//nothing to share
				return status.Success;//sharing nothing is success
			}
		}
	},
	ShareNSaveWebGL_CanShareBlob: function(blob, fileName, url, title, text) {
		const status = {Success: 0, CantShareError: 6, NotABlobError: 7, WebShareUnsupportedError: 8, WebShare2UnsupportedError: 9};
		var isCallingFromJS = false;
		if (typeof fileName === 'string') isCallingFromJS = true;
		
		if (!(blob instanceof Blob)) {//the blob is actually the property path that retrieves the blob
			if (blob) {//if trying to share a blob. If it's 0, no blob is trying to be shared.
				blob = !isCallingFromJS ? UTF8ToString(blob) : blob;
				if (typeof blob !== 'string') return status.NotABlobError;//not instance of blob and not a string, this is something else that is not a blob.
				function getBlob(propertyPath) {
					propertyPath = propertyPath;
					var object = (function(root) {
						return root === "Module" ? Module : root === "window" ? window : root === "document" ? document : undefined;
					})(propertyPath.split('.')[0]);
					if (!object) return undefined;
					propertyPath = propertyPath.split('.').slice(1).join('.');
					var resolvePath = function resolvePath(object, path, defaultValue) {
					  return path.split('.').reduce(function (o, p) {
						return o ? o[p] : defaultValue;
					  }, object);
					};
					return resolvePath(object, propertyPath);
				}
				blob = getBlob(blob);
				if (!(blob instanceof Blob)) return status.NotABlobError;//if it's not a blob, can't share. Return false
			}
		}
		
		if (blob) {
			if (!navigator.canShare || !navigator.canShare({files: [new File(["dummy"], "dummy.txt", {type: "text/plain"})]}))//trying to share a file and navigator.canShare is not supported
				return status.WebShare2UnsupportedError;
			else {
				const shareData = {};
				
				if (!isCallingFromJS) {
					fileName = fileName !== 0 ? UTF8ToString(fileName) : "file";
				} else {
					if (fileName === "")
						fileName = "file";
				}
				
				if (fileName.indexOf(".") === -1) fileName = fileName + "." + blob.type.split(";")[0].split("/")[1];
		
				if (blob) shareData.files = [new File([blob], fileName, {type: blob.type.split(";")[0]})];
				if (url) shareData.url = !isCallingFromJS ? UTF8ToString(url) : url;
				if (text) shareData.text = !isCallingFromJS ? UTF8ToString(text) : text;
				if (title) shareData.title = !isCallingFromJS ? UTF8ToString(title) : title;
				
				if (navigator.canShare(shareData)) return status.Success;
				else return status.CantShareError;
			}
		} else {
			if (url || text || title) {//if trying to share text only
				if (navigator.share) return status.Success;//they can share
				else return status.WebShareUnsupportedError;//they can't share
			} else {//nothing to share
				return status.Success;//sharing nothing is success
			}
		}
	}
});