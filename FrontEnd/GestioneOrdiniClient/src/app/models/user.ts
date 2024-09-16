export interface iUser {
  id: number,
  username: string,
  password: string
}

export interface iUserAuth {
  id: number,
  username: string,
  roleId: number
}

export interface iUserAuthWithRole {
  id: number,
  username: string,
  role: string
}
