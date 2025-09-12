import request from '../utils/request.js';

/** get constant routes */
export function fetchGetConstantRoutes() {
  return request({ url: '/route/getConstantRoutes' });
}

/** get user routes */
export function fetchGetUserRoutes() {
  return request({ url: '/route/getUserRoutes' });
}

/**
 * whether the route is exist
 *
 * @param routeName route name
 */
export function fetchIsRouteExist(routeName) {
  return request({ url: '/route/isRouteExist', params: { routeName } });
}
