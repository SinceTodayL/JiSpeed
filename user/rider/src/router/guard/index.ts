import type { Router } from 'vue-router';
// import { createProgressGuard } from './progress';
import { createDocumentTitleGuard } from './title';
import { createRouteGuard } from './route';

/**
 * Router guard
 *
 * @param router - Router instance
 */
export function createRouterGuard(router: Router) {
  // createProgressGuard(router);
  // Enable route guard to ensure routes are properly initialized
  createRouteGuard(router);
  createDocumentTitleGuard(router);
}
