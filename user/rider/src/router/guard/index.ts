import type { Router } from 'vue-router';
// import { createProgressGuard } from './progress';
import { createDocumentTitleGuard } from './title';

/**
 * Router guard
 *
 * @param router - Router instance
 */
export function createRouterGuard(router: Router) {
  // createProgressGuard(router);
  // Disable auth guard to skip login flow
  // createRouteGuard(router);
  createDocumentTitleGuard(router);
}
