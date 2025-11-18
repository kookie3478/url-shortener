document.getElementById("generateBtn").addEventListener("click", async () => {
  const button = document.getElementById("generateBtn");
  const spinner = document.getElementById("spinner");
  const btnText = document.getElementById("btnText");

  const url = document.getElementById("urlInput").value.trim();
  const mode = document.querySelector("input[name='mode']:checked").value;

  if (!url.trim()) {
    alert("Please enter a URL!");
    return;
  }

  // SHOW LOADING STATE
  button.classList.add("loading");
  spinner.classList.remove("hidden");

  try {
    const body = {
      url: url,
      generateQr: mode === "qr",
      expiryDays: 7,
      customAlias: null,
      baseUrl: null,
    };

    const response = await fetch("http://localhost:5114/api/shorten", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });

    const data = await response.json();
    const output = document.getElementById("output");
    output.innerHTML = "";

    if (data.shortUrl) {
      output.innerHTML += `<p>Short URL: <a href="${data.shortUrl}" target="_blank">${data.shortUrl}</a></p>`;
    }

    if (data.qrBase64 || data.qrCodeBase64) {
      const qr = data.qrBase64 || data.qrCodeBase64;
      output.innerHTML += `<p>QR Code:</p><img src="${qr}" width="200">`;
    }
  } catch (e) {
    alert("Something went wrong!");
    console.error(e);
  }

  // HIDE LOADING STATE
  button.classList.remove("loading");
  spinner.classList.add("hidden");
});
