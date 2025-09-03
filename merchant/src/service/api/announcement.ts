import { request } from '../request';

/**
 * Fetch active announcements for a specific target role
 * @param targetRole - Target role (e.g., 'merchant', 'customer', 'rider')
 * @param options - Optional query parameters for pagination
 */
export function fetchActiveAnnouncements(
  targetRole: string,
  options?: {
    size?: number;
    page?: number;
  }
) {
  const params = new URLSearchParams();
  
  if (options?.size !== undefined) params.append('size', options.size.toString());
  if (options?.page !== undefined) params.append('page', options.page.toString());

  const queryString = params.toString();
  const url = `/api/announcements/activate/${targetRole}${queryString ? `?${queryString}` : ''}`;

  return request<Api.Announcement.AnnouncementItem[]>({
    url,
    method: 'get'
  });
}

/**
 * Fetch all announcements for a specific target role
 * @param targetRole - Target role (e.g., 'merchant', 'customer', 'rider')
 * @param options - Optional query parameters for pagination
 */
export function fetchAllAnnouncements(
  targetRole: string,
  options?: {
    size?: number;
    page?: number;
  }
) {
  const params = new URLSearchParams();
  
  if (options?.size !== undefined) params.append('size', options.size.toString());
  if (options?.page !== undefined) params.append('page', options.page.toString());

  const queryString = params.toString();
  const url = `/api/announcements/all/${targetRole}${queryString ? `?${queryString}` : ''}`;

  return request<Api.Announcement.AnnouncementItem[]>({
    url,
    method: 'get'
  });
}

/**
 * Fetch single announcement detail by ID
 * @param announcementId - Announcement ID
 */
export function fetchAnnouncementDetail(announcementId: string) {
  return request<Api.Announcement.AnnouncementItem>({
    url: `/api/announcements/${announcementId}`,
    method: 'get'
  });
}
