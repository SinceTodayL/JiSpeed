using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // ����ģ���쳣��
    public class RiderExceptions
    {
        // ��������δ�ҵ��쳣
        // riderId: ���ֱ��
        // ����: δ�ҵ��쳣
        public static NotFoundException RiderNotFound(string riderId)
        {
            return new NotFoundException(ErrorCodes.RiderNotFound, $"���� (���: {riderId}) δ�ҵ�");
        }

        // ��������δ�ҵ��쳣(������)
        // name: ��������
        // ����: δ�ҵ��쳣
        public static NotFoundException RiderNotFoundByName(string name)
        {
            return new NotFoundException(ErrorCodes.RiderNotFound, $"���� (����: {name}) δ�ҵ�");
        }

        // ��������״̬�����쳣
        // riderId: ���ֱ��
        // currentStatus: ��ǰ״̬
        // requiredStatus: ����״̬
        // ����: ҵ���쳣
        public static BusinessException RiderStatusError(string riderId, string currentStatus, string requiredStatus)
        {
            return new BusinessException(ErrorCodes.RiderStatusError,
                $"���� (���: {riderId}) ��ǰ״̬ ({currentStatus}) ������˲�������Ҫ״̬: {requiredStatus}");
        }

        // �����������������쳣
        // riderId: ���ֱ��
        // currentOrders: ��ǰ������
        // ����: ҵ���쳣
        public static BusinessException RiderCapacityExceeded(string riderId, int currentOrders)
        {
            return new BusinessException(ErrorCodes.RiderCapacityExceeded,
                $"���� (���: {riderId}) ��ǰ���� {currentOrders} ���������޷����ܸ��ඩ��");
        }

        // �������ֲ��ڷ��������쳣
        // riderId: ���ֱ��
        // location: ��ǰλ��
        // ����: ҵ���쳣
        public static BusinessException RiderOutOfServiceArea(string riderId, string location)
        {
            return new BusinessException(ErrorCodes.RiderOutOfServiceArea,
                $"���� (���: {riderId}) ��ǰλ�� ({location}) ���ڷ���������");
        }

        // ���������˺��Ѵ����쳣
        // phoneNumber: ��ϵ�绰
        // ����: ҵ���쳣
        public static BusinessException RiderAlreadyExists(string phoneNumber)
        {
            return new BusinessException(ErrorCodes.RiderAlreadyExists,
                $"�����˺� (�绰: {phoneNumber}) �Ѵ���");
        }

        // ����������֤ʧ���쳣
        // riderId: ���ֱ��
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException RiderAuthenticationFailed(string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.RiderAuthenticationFailed,
                $"���� (���: {riderId}) ��֤ʧ��: {reason}");
        }

        // ��������λ�ø���ʧ���쳣
        // riderId: ���ֱ��
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException RiderLocationUpdateFailed(string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.RiderLocationUpdateFailed,
                $"���� (���: {riderId}) λ�ø���ʧ��: {reason}");
        }

        // �����������ֹ����쳣
        // riderId: ���ֱ��
        // rating: ��ǰ����
        // minimumRating: ���Ҫ������
        // ����: ҵ���쳣
        public static BusinessException RiderRatingTooLow(string riderId, decimal rating, decimal minimumRating)
        {
            return new BusinessException(ErrorCodes.RiderRatingTooLow,
                $"���� (���: {riderId}) ��ǰ���� ({rating}) ����ϵͳҪ�� ({minimumRating})");
        }

        // ����������Ϣ���쳣
        // riderId: ���ֱ��
        // ����: ҵ���쳣
        public static BusinessException RiderOnBreak(string riderId)
        {
            return new BusinessException(ErrorCodes.RiderOnBreak,
                $"���� (���: {riderId}) ��ǰ������Ϣ״̬���޷��ӵ�");
        }

        // ���������豸�����쳣
        // riderId: ���ֱ��
        // ����: ҵ���쳣
        public static BusinessException RiderDeviceOffline(string riderId)
        {
            return new BusinessException(ErrorCodes.RiderDeviceOffline,
                $"���� (���: {riderId}) �豸���ߣ�������������");
        }

        // ����������Ϣ�����쳣
        // riderId: ���ֱ��
        // vehicleNumber: ���ƺ�
        // reason: ����ԭ��
        // ����: ҵ���쳣
        public static BusinessException RiderVehicleError(string riderId, string vehicleNumber, string reason)
        {
            return new BusinessException(ErrorCodes.RiderDeviceOffline,
                $"���� (���: {riderId}) ���� (����: {vehicleNumber}) ��Ϣ����: {reason}");
        }
    }
}
