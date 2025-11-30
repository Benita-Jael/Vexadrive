import { environment } from '../../../environments/environment';

// Use environment.apiBaseUrl so the backend URL can be configured per environment.
export const API_BASE_AUTH_URL = `${environment.apiBaseUrl}/api/auth`;
export const TOKEN_KEY = 'jwt_token';
export const USER_KEY = 'current_user';
