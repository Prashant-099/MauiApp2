window.capturePhoto = async function () {
    const video = document.createElement("video");
    const stream = await navigator.mediaDevices.getUserMedia({ video: true });
    video.srcObject = stream;
    await video.play();

    const canvas = document.createElement("canvas");
    canvas.width = 320;
    canvas.height = 240;
    const context = canvas.getContext("2d");
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    stream.getTracks().forEach(t => t.stop());
    return canvas.toDataURL("image/png");
};
