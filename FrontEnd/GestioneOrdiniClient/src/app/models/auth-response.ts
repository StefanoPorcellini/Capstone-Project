import { iUserAuth } from "./user";

export interface iAuthResponse {
  token: string,
  expires: string,
  userAuth: iUserAuth
}
