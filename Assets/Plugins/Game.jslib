mergeInto(LibraryManager.library, {

  OpenDeepLink: function (str) {
    
    const url = UTF8ToString(str); // Convert the C# string to JavaScript string

    // Redirect to the specified deep link if it matches the custom scheme
      window.location.href = url;

      console.log("Called URL");
  },

});