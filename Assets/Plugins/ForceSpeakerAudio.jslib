mergeInto(LibraryManager.library, {
    ForceAudioToSpeaker: function() {
        if (navigator.userAgent.includes("iPhone") || navigator.userAgent.includes("iPad")) {
            const context = new (window.AudioContext || window.webkitAudioContext)();
            const source = context.createBufferSource();
            const gainNode = context.createGain();

            source.connect(gainNode).connect(context.destination);

            // Creating a silent sound buffer to initialize the audio context
            const buffer = context.createBuffer(1, context.sampleRate, context.sampleRate);
            source.buffer = buffer;
            source.loop = false;
            source.start(0);

            console.log("Audio initialized to use iOS speaker.");
        } else {
            console.log("This function is designed for iOS devices only.");
        }
    }
});
