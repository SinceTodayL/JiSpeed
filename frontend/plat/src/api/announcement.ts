import { get, post } from '../utils/request.js';

/**
 * 根据当前日期和适用范围返回可用的公告list
 * @param targetRole - 目标角色 (all, user, merchant, rider)
 */
export function getActiveAnnouncements(targetRole = 'all') {
  console.log(`获取有效公告列表，目标角色: ${targetRole}`);
  return get(`/api/announcements/activate/${targetRole}`);
}

/**
 * 管理员发布公告
 * @param adminId - 管理员ID
 * @param announcementData - 公告数据
 */
export function postAnnouncement(adminId, announcementData) {
  console.log(`发布公告，管理员ID: ${adminId}`, announcementData);
  return post(`/api/admin/${adminId}/announcements`, announcementData);
}

/**
 * 获取公告详情
 * @param announcementId - 公告ID
 */
export function getAnnouncementDetail(announcementId) {
  console.log(`获取公告详情，ID: ${announcementId}`);
  return get(`/api/announcements/${announcementId}`);
}

/**
 * 删除公告
 * @param adminId - 管理员ID
 * @param announcementId - 公告ID
 */
export function deleteAnnouncement(adminId, announcementId) {
  console.log(`删除公告，管理员ID: ${adminId}, 公告ID: ${announcementId}`);
  return post(`/api/admin/${adminId}/announcements/${announcementId}/delete`);
}

/**
 * 更新公告
 * @param adminId - 管理员ID
 * @param announcementId - 公告ID
 * @param updateData - 更新数据
 */
export function updateAnnouncement(adminId, announcementId, updateData) {
  console.log(`更新公告，管理员ID: ${adminId}, 公告ID: ${announcementId}`, updateData);
  return post(`/api/admin/${adminId}/announcements/${announcementId}`, updateData);
}