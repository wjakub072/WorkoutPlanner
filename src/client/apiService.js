import router from '../router/index';
import { useAuthStore } from '../stores/authStore.js';
import { apiUrl } from '../apiUrl';
const baseUrl = apiUrl;

export const ApiService = {
  isAccessTokenValid() {
    const authStore = useAuthStore();

    return (
      authStore.accessToken?.token &&
      new Date() < new Date(authStore.accessToken?.tokenExpires)
    );
  },

  async request(method, url, queryParams = {}, body = null) {
    if (
      url != 'auth/sign-in' &&
      url != 'auth/sign-out' &&
      url != 'auth/sign-up' &&
      url != 'auth/password-token' &&
      url != 'auth/password' &&
      !this.isAccessTokenValid()
    ) {
      try {
        await this.refreshAccessToken();
      } catch {
        console.log('Refresh failed pushing to login...');
        router.push({ name: 'sign-in' });
      }
    }

    const filteredQueryParams = Object.fromEntries(
      Object.entries(queryParams).filter(([_, value]) => value !== null),
    );

    const params = new URLSearchParams(filteredQueryParams).toString();
    const requestUrl = params
      ? `${baseUrl}/${url}?${params}`
      : `${baseUrl}/${url}`;
    const authStore = useAuthStore();

    const response = await fetch(requestUrl, {
      method,
      body: body && JSON.stringify(body),
      headers: {
        'Content-Type': 'application/json',
        Authorization: 'Bearer ' + authStore.accessToken?.token,
      },
    });

    if (!response.ok) {
      if (response.status === 404) {
        router.push({ name: 'not-found' });
        return;
      }

      let errorMessage = `Error ${response.status}: ${response.statusText}`;
      let responseBody;

      const contentType = response.headers.get('content-type');
      if (contentType) {
        if (
          contentType.includes('application/json') ||
          contentType.includes('application/problem+json')
        ) {
          responseBody = await response.json();
          errorMessage = responseBody.message || errorMessage;
        } else if (contentType.includes('text/plain')) {
          responseBody = await response.text();
          errorMessage = responseBody || errorMessage;
        }
      }

      throw new Error(errorMessage);
    }

    let responseBody;
    const contentType = response.headers.get('content-type');
    if (contentType) {
      if (
        contentType.includes('application/json') ||
        contentType.includes('application/problem+json')
      ) {
        responseBody = await response.json();
      } else if (contentType.includes('text/plain')) {
        responseBody = await response.text();
      } else if (contentType.includes('application/pdf')) {
        responseBody = await response.arrayBuffer();
      } else if (contentType.includes('image/png')) {
        responseBody = await response.blob();
      }
    }

    return responseBody;
  },

  async get(url, queryParams = {}) {
    return await this.request('GET', url, queryParams);
  },

  async post(url, queryParams = {}, body = null) {
    return await this.request('POST', url, queryParams, body);
  },

  async put(url, queryParams = {}, body = null) {
    return await this.request('PUT', url, queryParams, body);
  },

  async delete(url, queryParams = {}) {
    return await this.request('DELETE', url, queryParams);
  },
};
