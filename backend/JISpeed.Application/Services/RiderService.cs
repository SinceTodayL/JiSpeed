using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JISpeed.Core.Entities.Rider;
using JISpeed.Core.Interfaces.IRepositories;
using JISpeed.Core.Interfaces.IServices;
using JISpeed.Core.Exceptions;
using JISpeed.Core.Constants;
using Microsoft.Extensions.Logging;

namespace JISpeed.Application.Services
{
    // ���ַ���ʵ��
    public class RiderService : IRiderService
    {
        private readonly IRiderRepository _riderRepository;
        private readonly ILogger<RiderService> _logger;

        public RiderService(IRiderRepository riderRepository, ILogger<RiderService> logger)
        {
            _riderRepository = riderRepository;
            _logger = logger;
        }

        // ����ID��ȡ������Ϣ
        // <param name="riderId">����ID</param>
        // <returns>����ʵ��</returns>
        public async Task<Rider> GetRiderByIdAsync(string riderId)
        {
            try
            {
                _logger.LogInformation("��ʼ��ȡ������Ϣ, RiderId: {RiderId}", riderId);

                if (string.IsNullOrWhiteSpace(riderId))
                {
                    _logger.LogWarning("����ID����Ϊ��");
                    throw CommonExceptions.ValidationFailed("riderId", "����Ϊ��");
                }

                var rider = await _riderRepository.GetByIdAsync(riderId);

                if (rider == null)
                {
                    _logger.LogWarning("���ֲ�����, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                _logger.LogInformation("�ɹ���ȡ������Ϣ, RiderId: {RiderId}, Name: {Name}",
                    riderId, rider.Name);

                return rider;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "��ȡ������Ϣʱ�����쳣, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"��ȡ������Ϣʧ��: {ex.Message}");
            }
        }

        // ����������
        // <param name="rider">����ʵ��</param>
        // <returns>����������ʵ��</returns>
        public async Task<Rider> CreateRiderAsync(Rider rider)
        {
            try
            {
                _logger.LogInformation("��ʼ��������, Name: {Name}, Phone: {Phone}",
                    rider.Name, rider.PhoneNumber);

                // ������֤
                if (rider == null)
                {
                    throw CommonExceptions.ValidationFailed("rider", "������Ϣ����Ϊ��");
                }

                if (string.IsNullOrWhiteSpace(rider.Name))
                {
                    throw CommonExceptions.ValidationFailed("name", "�������Ʋ���Ϊ��");
                }

                if (string.IsNullOrWhiteSpace(rider.PhoneNumber))
                {
                    throw CommonExceptions.ValidationFailed("phoneNumber", "�����ֻ��Ų���Ϊ��");
                }

                // ����ֻ����Ƿ��Ѵ���
                var exists = await _riderRepository.ExistsByPhoneAsync(rider.PhoneNumber);
                if (exists)
                {
                    _logger.LogWarning("�����ֻ����Ѵ���, Phone: {Phone}", rider.PhoneNumber);
                    throw RiderExceptions.RiderAlreadyExists(rider.PhoneNumber);
                }

                // ��������ID
                if (string.IsNullOrEmpty(rider.RiderId))
                {
                    rider.RiderId = Guid.NewGuid().ToString("N");
                }

                // ����������Ϣ
                await _riderRepository.AddAsync(rider);

                _logger.LogInformation("���ִ����ɹ�, RiderId: {RiderId}, Name: {Name}",
                    rider.RiderId, rider.Name);

                return rider;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is BusinessException))
            {
                _logger.LogError(ex, "��������ʱ�����쳣, Name: {Name}, Phone: {Phone}",
                    rider.Name, rider.PhoneNumber);
                throw CommonExceptions.GeneralError($"��������ʧ��: {ex.Message}");
            }
        }

        // ����������Ϣ
        // <param name="rider">���µ�����ʵ��</param>
        // <returns>���º������ʵ��</returns>
        public async Task<Rider> UpdateRiderAsync(Rider rider)
        {
            try
            {
                _logger.LogInformation("��ʼ����������Ϣ, RiderId: {RiderId}", rider.RiderId);

                // ������֤
                if (rider == null || string.IsNullOrWhiteSpace(rider.RiderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "����ID����Ϊ��");
                }

                // ��������Ƿ����
                var existingRider = await _riderRepository.GetByIdAsync(rider.RiderId);
                if (existingRider == null)
                {
                    _logger.LogWarning("���ֲ�����, RiderId: {RiderId}", rider.RiderId);
                    throw RiderExceptions.RiderNotFound(rider.RiderId);
                }

                // ����������Ϣ
                await _riderRepository.UpdateAsync(rider);

                _logger.LogInformation("������Ϣ���³ɹ�, RiderId: {RiderId}", rider.RiderId);

                return rider;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "����������Ϣʱ�����쳣, RiderId: {RiderId}", rider.RiderId);
                throw CommonExceptions.GeneralError($"����������Ϣʧ��: {ex.Message}");
            }
        }

        // ��ȡ���ֵĶ��������б�
        // <param name="riderId">����ID</param>
        // <param name="status">����״̬ɸѡ����ѡ��</param>
        // <returns>���������б�</returns>
        public async Task<IEnumerable<Assignment>> GetRiderAssignmentsAsync(string riderId, int? status = null)
        {
            try
            {
                _logger.LogInformation("��ʼ��ȡ���ֶ��������б�, RiderId: {RiderId}, Status: {Status}",
                    riderId, status.HasValue ? status.Value.ToString() : "ȫ��");

                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "����ID����Ϊ��");
                }

                // ��������Ƿ����
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("���ֲ�����, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // ��ȡ���������б�
                var assignments = await _riderRepository.GetAssignmentsAsync(riderId);

                // ����״̬ɸѡ
                if (status.HasValue)
                {
                    assignments = assignments.Where(a => a.AcceptedStatus == status.Value);
                }

                _logger.LogInformation("�ɹ���ȡ���ֶ��������б�, RiderId: {RiderId}, Count: {Count}",
                    riderId, assignments.Count());

                return assignments;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException))
            {
                _logger.LogError(ex, "��ȡ���ֶ��������б�ʱ�����쳣, RiderId: {RiderId}", riderId);
                throw CommonExceptions.GeneralError($"��ȡ���ֶ��������б�ʧ��: {ex.Message}");
            }
        }

        // ���¶�������״̬���ӵ�/�ܵ���
        // <param name="riderId">����ID</param>
        // <param name="assignId">����ID</param>
        // <param name="acceptedStatus">�ӵ�״̬</param>
        // <returns>���º�Ķ�������</returns>
        public async Task<Assignment> UpdateAssignmentStatusAsync(string riderId, string assignId, int acceptedStatus)
        {
            try
            {
                _logger.LogInformation("��ʼ���¶�������״̬, RiderId: {RiderId}, AssignId: {AssignId}, Status: {Status}",
                    riderId, assignId, acceptedStatus);

                // ������֤
                if (string.IsNullOrWhiteSpace(riderId))
                {
                    throw CommonExceptions.ValidationFailed("riderId", "����ID����Ϊ��");
                }

                if (string.IsNullOrWhiteSpace(assignId))
                {
                    throw CommonExceptions.ValidationFailed("assignId", "��������ID����Ϊ��");
                }

                // ��������Ƿ����
                var riderExists = await _riderRepository.GetByIdAsync(riderId) != null;
                if (!riderExists)
                {
                    _logger.LogWarning("���ֲ�����, RiderId: {RiderId}", riderId);
                    throw RiderExceptions.RiderNotFound(riderId);
                }

                // ��ȡ��������
                var assignment = await _riderRepository.GetAssignmentByIdAsync(assignId);
                if (assignment == null)
                {
                    _logger.LogWarning("�������䲻����, AssignId: {AssignId}", assignId);
                    throw new NotFoundException(ErrorCodes.ResourceNotFound, $"�������䲻���ڣ�ID: {assignId}");
                }

                // ��֤���������Ƿ����ڸ�����
                if (assignment.RiderId != riderId)
                {
                    _logger.LogWarning("�������䲻���ڸ�����, RiderId: {RiderId}, AssignId: {AssignId}",
                        riderId, assignId);
                    throw AuthExceptions.Forbidden(riderId, $"�������� (ID: {assignId})");
                }

                // ��֤����״̬�Ƿ��������
                if (assignment.AcceptedStatus != 0) // ����0��ʾδ����
                {
                    _logger.LogWarning("��������״̬���������, AssignId: {AssignId}, CurrentStatus: {CurrentStatus}",
                        assignId, assignment.AcceptedStatus);
                    throw OrderExceptions.OrderStatusError(assignId, assignment.AcceptedStatus, 0);
                }

                // ���¶�������״̬
                assignment.AcceptedStatus = acceptedStatus;
                assignment.AcceptedAt = DateTime.Now;

                await _riderRepository.UpdateAssignmentAsync(assignment);

                _logger.LogInformation("��������״̬���³ɹ�, AssignId: {AssignId}, Status: {Status}",
                    assignId, acceptedStatus);

                return assignment;
            }
            catch (Exception ex) when (!(ex is ValidationException || ex is NotFoundException || ex is BusinessException))
            {
                _logger.LogError(ex, "���¶�������״̬ʱ�����쳣, RiderId: {RiderId}, AssignId: {AssignId}",
                    riderId, assignId);
                throw CommonExceptions.GeneralError($"���¶�������״̬ʧ��: {ex.Message}");
            }
        }
    }
}
