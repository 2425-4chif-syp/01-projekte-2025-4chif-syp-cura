export const environment = {
  production: false,
  apiUrl: 'https://vm12.htl-leonding.ac.at/api',     // ✅ Backend auf Server
  keycloak: {
    url: 'https://vm12.htl-leonding.ac.at/auth',    // ✅ Keycloak auf Server
    realm: 'cura',
    clientId: 'cura-frontend'
  }
};