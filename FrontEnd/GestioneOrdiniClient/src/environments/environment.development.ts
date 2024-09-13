export const environment = {
  baseUrl: 'https://localhost:7147/api',
  userapi: {
    create: 'User/Create',
    login: 'User/Login',
    all: 'User/GetAll',
    getUserById: (userId: string) => `/getUserById/${userId}`,
    update: (userId: string) => `/update/${userId}`,
    delete: (userId: string) => `/delete/${userId}`,
    roles: 'User/GetUsers',
  },
};
