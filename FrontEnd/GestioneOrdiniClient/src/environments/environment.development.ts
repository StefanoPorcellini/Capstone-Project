export const environment = {
  baseUrl: 'https://localhost:7147/api',
  userapi: {
    create: 'User/Create',
    login: 'User/Login',
    all: 'User/all',
    getUserById: (userId: number) => `User/getUserById/${userId}`,
    update: (userId: number) => `User/update/${userId}`,
    delete: (userId: number) => `User/delete/${userId}`,
    roles: 'User/roles',
  },
  standardApi: {
    create: (url:string) => `${url}/create`,
    getAll: (url:string) => `${url}/getAll`,
    getById: (url:string, id: number) => `${url}/getById/${id}`,
    update: (url:string, id: number) => `${url}/update/${id}`,
    delete: (url:string, id: number) => `${url}/delete/${id}`
  },
  orderApi: {
    assign: (url:string, id: number) => `${url}/${id}/assign`,
    status: (url:string, id: number) => `${url}/${id}/status`,
  }
};
