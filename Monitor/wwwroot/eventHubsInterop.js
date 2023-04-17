window.eventHubsInterop = {
    createSasToken: async (uri, keyName, key, expiresInMins) => {
        if (!uri || !keyName || !key) {
            throw "Missing required parameter";
        }
        resourceUri = "https://" + uri;
        const encoded = encodeURIComponent(resourceUri);
        const ttl = Math.ceil(Date.now() / 1000 + expiresInMins * 60);
        const toBeSigned = encoded + '\n' + ttl;
        const encoder = new TextEncoder();

        const keyBytes = encoder.encode(key);

        const toBeSignedBytes = encoder.encode(toBeSigned);

        const cryptoKey = await window.crypto.subtle.importKey(
            "raw",
            keyBytes,
            { name: "HMAC", hash: "SHA-256" },
            false,
            ["sign"]
        );
        const signature = await crypto.subtle.sign("HMAC", cryptoKey, toBeSignedBytes);

        // Converting signature ArrayBuffer to base64 and then URL encoding

        const base64Signature = btoa(String.fromCharCode(...new Uint8Array(signature)));
        const encodedSignature = encodeURIComponent(base64Signature);

        return 'SharedAccessSignature sr=' + encoded + '&sig=' +
            encodedSignature + '&se=' + ttl + '&skn=' + keyName;
    }
};