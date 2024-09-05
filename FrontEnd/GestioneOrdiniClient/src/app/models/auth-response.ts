import { iAuthData } from './auth-data';
import { iUser } from './user';
export interface iAuthResponse {
  accessToken:string,
  user: iUser,
  expires: Date
}
