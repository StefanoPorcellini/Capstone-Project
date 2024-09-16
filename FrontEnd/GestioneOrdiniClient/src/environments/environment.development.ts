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
};
