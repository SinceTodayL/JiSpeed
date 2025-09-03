import { request } from '../request';

/**
 * Get all dishes for a specific merchant
 * @param merchantId - Merchant ID
 * @param options - Optional query parameters for filtering and pagination
 */
export function fetchGetAllDishes(
  merchantId: string,
  options?: {
    categoryId?: string;
    orderByRating?: boolean;
    orderByHighPrice?: boolean;
    orderByLowPrice?: boolean;
    size?: number;
    page?: number;
  }
) {
  const params = new URLSearchParams();
  
  if (options?.categoryId) params.append('categoryId', options.categoryId);
  if (options?.orderByRating !== undefined) params.append('orderByRating', options.orderByRating.toString());
  if (options?.orderByHighPrice !== undefined) params.append('orderByHighPrice', options.orderByHighPrice.toString());
  if (options?.orderByLowPrice !== undefined) params.append('orderByLowPrice', options.orderByLowPrice.toString());
  if (options?.size !== undefined) params.append('size', options.size.toString());
  if (options?.page !== undefined) params.append('page', options.page.toString());

  const queryString = params.toString();
  const url = `/api/merchants/${merchantId}/dishes${queryString ? `?${queryString}` : ''}`;

  return request<Api.Goods.DishItem[]>({
    url,
    method: 'get'
  });
}

/**
 * Get single dish details
 * @param merchantId - Merchant ID
 * @param dishId - Dish ID
 */
export function fetchDishDetail(merchantId: string, dishId: string) {
  return request<Api.Goods.DishItem>({
    url: `/api/merchants/${merchantId}/dish/${dishId}`,
    method: 'get'
  });
}

/**
 * Create a new dish
 * @param merchantId - Merchant ID
 * @param dishData - Dish creation data
 */
export function createDish(merchantId: string, dishData: Api.Goods.CreateDishRequest) {
  return request<boolean>({
    url: `/api/merchants/${merchantId}/addNewDish`,
    method: 'post',
    data: dishData
  });
}

/**
 * Update an existing dish
 * @param merchantId - Merchant ID
 * @param dishId - Dish ID to update
 * @param dishData - Dish update data
 */
export function updateDish(merchantId: string, dishId: string, dishData: Api.Goods.UpdateDishRequest) {
  return request<boolean>({
    url: `/api/merchants/${merchantId}/${dishId}`,
    method: 'patch',
    data: dishData
  });
}

/**
 * Delete a dish
 * @param merchantId - Merchant ID
 * @param dishId - Dish ID to delete
 */
export function deleteDish(merchantId: string, dishId: string) {
  return request<boolean>({
    url: `/api/merchants/${merchantId}/${dishId}`,
    method: 'delete'
  });
}

/**
 * Batch delete multiple dishes
 * @param merchantId - Merchant ID
 * @param dishIds - Array of dish IDs to delete
 */
export function batchDeleteDishes(merchantId: string, dishIds: string[]) {
  return Promise.all(
    dishIds.map(dishId => deleteDish(merchantId, dishId))
  );
} 