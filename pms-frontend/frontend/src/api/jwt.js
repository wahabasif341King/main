// JWT ka payload decode karta hai (sirf read karne ke liye, verify nahi karta —
// verification backend already karta hai)
export function decodeToken(token) {
  try {
    const payload = token.split('.')[1];
    const decoded = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
    return {
      id: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || decoded.nameid || decoded.sub,
      username: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname'] || decoded.unique_name,
      role: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'] || decoded.role,
    };
  } catch {
    return null;
  }
}
