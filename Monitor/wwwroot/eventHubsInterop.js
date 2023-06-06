window.eventHubsInterop = {
    createSasToken: async (resourceUri, keyName, key, expiresInMins) => {
        const encodedResourceUri = encodeURIComponent(resourceUri);
        const expiresInSeconds = Math.ceil(Date.now() / 1000 + expiresInMins * 60);

        const keyBytes = new Uint8Array([...atob(key)].map(c => c.charCodeAt(0)));
        const digest = await crypto.subtle.digest("SHA-256", keyBytes);
        const signatureBytes = new Uint8Array(digest);

        const signature = btoa(String.fromCharCode(...signatureBytes));
        const sasToken = `SharedAccessSignature sr=${encodedResourceUri}&sig=${encodeURIComponent(signature)}&se=${expiresInSeconds}&skn=${keyName}`;

        return sasToken;
    }
};