import { ApiService } from '../services/apiService';
import { useAuthStore } from '../stores/authStore';

export const AuthService = {
  async login(userName, password) {
    const body = {
      Email: userName,
      Password: password,
    };

    const data = await ApiService.post(`auth/sign-in`, {}, body);

    const authStore = useAuthStore();
    authStore.setUser(data.user);
    authStore.setAccessToken(data.accessToken);
  },
  async register(userName, email, password) {
    const body = {
      UserName: userName,
      Email: email,
      Password: password,
    };

    const data = await ApiService.post(`auth/sign-up`, {}, body);
    return data != null;
  },
  async logout() {
    await ApiService.post(`auth/sign-out`, {}, null);

    const authStore = useAuthStore();
    authStore.clearUser();
    authStore.clearAccessToken();
  },

  async getCurrentUser() {
    try {
      return await ApiService.get(endpoint, {});
    } catch {
      return;
    }
  }, // ==========================================================================================================
  async getPasswordResetToken(email) {
    return await ApiService.get('auth/password-token', {
      Email: email,
    });
  },
  async newPassword(email, resetToken, password) {
    const body = {
      Email: email,
      ResetToken: resetToken,
      Password: password,
    };

    const data = await ApiService.post(`auth/password`, {}, body);
    return data != null;
  },
  async getEmailResetToken(email) {
    return await ApiService.get('auth/email-token', { NewEmail: email });
  },
  async tryChangeEmail(inputCode, newEmail) {
    const body = {
      ResetToken: inputCode,
      NewEmail: newEmail,
    };

    const data = await ApiService.post(`auth/email`, {}, body);
    return data;
  },
};
