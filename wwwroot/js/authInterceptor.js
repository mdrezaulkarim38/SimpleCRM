(function () {
    debugger;
    const originalFetch = window.fetch;

    window.fetch = async function (...args) {
        const token = sessionStorage.getItem('token'); // Get the JWT token from session storage

        if (token) {
            // Ensure the second argument (options) is an object
            args[1] = {
                ...args[1],
                headers: {
                    ...args[1]?.headers, // Existing headers
                    'Authorization': `Bearer ${token}` // Add Authorization header
                }
            };
        }

        try {
            const response = await originalFetch.apply(this, args); // Call the original fetch
            if (!response.ok) {
                // Handle 401 Unauthorized error
                if (response.status === 401) {
                    console.error('Unauthorized access - redirecting to login');
                    // Optional: Redirect to login page or handle accordingly
                }
            }
            return response;
        } catch (error) {
            console.error('Fetch error:', error);
            throw error; // Re-throw the error for further handling if needed
        }
    };
})();
