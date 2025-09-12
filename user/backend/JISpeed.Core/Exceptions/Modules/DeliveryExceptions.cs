using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // ����ģ���쳣��
    public class DeliveryExceptions
    {
        // ����������Ϣδ�ҵ��쳣
        // orderId: ����ID
        // ����: δ�ҵ��쳣
        public static NotFoundException DeliveryNotFound(string orderId)
        {
            return new NotFoundException(ErrorCodes.DeliveryNotFound, $"������Ϣ (����ID: {orderId}) δ�ҵ�");
        }

        // ��������״̬�����쳣
        // orderId: ����ID
        // currentStatus: ��ǰ״̬
        // requiredStatus: ����״̬
        // ����: ҵ���쳣
        public static BusinessException DeliveryStatusError(string orderId, int currentStatus, int requiredStatus)
        {
            return new BusinessException(ErrorCodes.DeliveryStatusError,
                $"���� (����ID: {orderId}) ��ǰ״̬ ({currentStatus}) ������˲�������Ҫ״̬: {requiredStatus}");
        }

        // ������������֧���쳣
        // addressId: ��ַID
        // detailedAddress: ��ϸ��ַ
        // ����: ҵ���쳣
        public static BusinessException DeliveryAreaNotSupported(string addressId, string detailedAddress)
        {
            return new BusinessException(ErrorCodes.DeliveryAreaNotSupported,
                $"��ַ (ID: {addressId}, {detailedAddress}) �������ͷ���Χ��");
        }

        // ��������·�߹滮ʧ���쳣
        // orderId: ����ID
        // riderId: ����ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException DeliveryRouteFailed(string orderId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.DeliveryRouteFailed,
                $"���� (ID: {riderId}) ���Ͷ��� (ID: {orderId}) ·�߹滮ʧ��: {reason}");
        }

        // �������;��볬����Χ�쳣
        // orderId: ����ID
        // distance: ���;���
        // maxDistance: ������;���
        // ����: ҵ���쳣
        public static BusinessException DeliveryDistanceExceeded(string orderId, decimal distance, decimal maxDistance)
        {
            return new BusinessException(ErrorCodes.DeliveryDistanceExceeded,
                $"���� (ID: {orderId}) ���;��� ({distance}km) ����������ͷ�Χ ({maxDistance}km)");
        }

        // ��������ʱ���ͻ�쳣
        // assignId: ������
        // riderId: ����ID
        // conflictAssignId: ��ͻ�ķ�����
        // ����: ҵ���쳣
        public static BusinessException DeliveryTimeConflict(string assignId, string riderId, string conflictAssignId)
        {
            return new BusinessException(ErrorCodes.DeliveryTimeConflict,
                $"���� (ID: {riderId}) ���������� (ID: {assignId}) ������ (ID: {conflictAssignId}) ʱ���ͻ");
        }

        // ���������ӳ��쳣
        // orderId: ����ID
        // riderId: ����ID
        // expectedTime: Ԥ���ʹ�ʱ��
        // estimatedTime: �����ʹ�ʱ��
        // ����: ҵ���쳣
        public static BusinessException DeliveryDelayed(string orderId, string riderId, DateTime expectedTime, DateTime estimatedTime)
        {
            return new BusinessException(ErrorCodes.DeliveryDelayed,
                $"���� (ID: {riderId}) ���Ͷ��� (ID: {orderId}) �ӳ٣�Ԥ���ʹ�ʱ��: {expectedTime:yyyy-MM-dd HH:mm:ss}��" +
                $"�����ʹ�ʱ��: {estimatedTime:yyyy-MM-dd HH:mm:ss}");
        }

        // ��������ȡ��ʧ���쳣
        // assignId: ������
        // riderId: ����ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException DeliveryCancelFailed(string assignId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.DeliveryCancelFailed,
                $"���� (ID: {riderId}) ���������� (ID: {assignId}) ȡ��ʧ��: {reason}");
        }

        // �����������ȷ��ʧ���쳣
        // assignId: ������
        // riderId: ����ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException DeliveryCompletionFailed(string assignId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.DeliveryCompletionFailed,
                $"���� (ID: {riderId}) ���������� (ID: {assignId}) ���ȷ��ʧ��: {reason}");
        }
    }
}
