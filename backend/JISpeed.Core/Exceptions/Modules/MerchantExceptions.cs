using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // �̼�ģ���쳣��
    public class MerchantExceptions
    {
        // �����̼�δ�ҵ��쳣
        // merchantId: �̼�ID
        // ����: δ�ҵ��쳣
        public static NotFoundException MerchantNotFound(string merchantId)
        {
            return new NotFoundException(ErrorCodes.MerchantNotFound, $"�̼� (ID: {merchantId}) δ�ҵ�");
        }

        // �����̼�δ�ҵ��쳣(�����̼�����)
        // merchantName: �̼�����
        // ����: δ�ҵ��쳣
        public static NotFoundException MerchantNotFoundByName(string merchantName)
        {
            return new NotFoundException(ErrorCodes.MerchantNotFound, $"�̼� (����: {merchantName}) δ�ҵ�");
        }

        // �����̼�״̬�����쳣
        // merchantId: �̼�ID
        // currentStatus: ��ǰ״̬
        // requiredStatus: ����״̬
        // ����: ҵ���쳣
        public static BusinessException MerchantStatusError(string merchantId, int currentStatus, int requiredStatus)
        {
            string currentStatusText = GetMerchantStatusText(currentStatus);
            string requiredStatusText = GetMerchantStatusText(requiredStatus);

            return new BusinessException(ErrorCodes.MerchantStatusError,
                $"�̼� (ID: {merchantId}) ��ǰ״̬ ({currentStatusText}) ������˲�������Ҫ״̬: {requiredStatusText}");
        }

        // �����̼��ѹر��쳣
        // merchantId: �̼�ID
        // ����: ҵ���쳣
        public static BusinessException MerchantClosed(string merchantId)
        {
            return new BusinessException(ErrorCodes.MerchantClosed,
                $"�̼� (ID: {merchantId}) �ѹرգ���ǰ�����ܶ���");
        }

        // �����̼���֤ʧ���쳣
        // merchantId: �̼�ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException MerchantAuthenticationFailed(string merchantId, string reason)
        {
            return new BusinessException(ErrorCodes.MerchantAuthenticationFailed,
                $"�̼� (ID: {merchantId}) ��֤ʧ��: {reason}");
        }

        // ������Ʒδ�ҵ��쳣
        // dishId: ��ƷID
        // ����: δ�ҵ��쳣
        public static NotFoundException ProductNotFound(string dishId)
        {
            return new NotFoundException(ErrorCodes.ProductNotFound, $"��Ʒ (ID: {dishId}) δ�ҵ�");
        }

        // ������Ʒδ�ҵ��쳣(���ݲ�Ʒ����)
        // dishName: ��Ʒ����
        // ����: δ�ҵ��쳣
        public static NotFoundException ProductNotFoundByName(string dishName)
        {
            return new NotFoundException(ErrorCodes.ProductNotFound, $"��Ʒ (����: {dishName}) δ�ҵ�");
        }

        // ������Ʒ���¼��쳣
        // dishId: ��ƷID
        // dishName: ��Ʒ����
        // ����: ҵ���쳣
        public static BusinessException ProductUnavailable(string dishId, string dishName)
        {
            return new BusinessException(ErrorCodes.ProductUnavailable,
                $"��Ʒ (ID: {dishId}, ����: {dishName}) ���¼ܣ���ǰ���ɹ���");
        }

        // ������Ʒ��治���쳣
        // dishId: ��ƷID
        // dishName: ��Ʒ����
        // requestedQuantity: ��������
        // availableQuantity: ��������
        // ����: ҵ���쳣
        public static BusinessException ProductOutOfStock(string dishId, string dishName, int requestedQuantity, int availableQuantity)
        {
            return new BusinessException(ErrorCodes.ProductOutOfStock,
                $"��Ʒ (ID: {dishId}, ����: {dishName}) ��治�㣬����: {requestedQuantity}������: {availableQuantity}");
        }

        // �����̼�Ӫҵʱ������쳣
        // merchantId: �̼�ID
        // merchantName: �̼�����
        // currentTime: ��ǰʱ��
        // ����: ҵ���쳣
        public static BusinessException MerchantBusinessHoursError(string merchantId, string merchantName, string currentTime)
        {
            return new BusinessException(ErrorCodes.MerchantBusinessHoursError,
                $"�̼� (ID: {merchantId}, ����: {merchantName}) ��ǰ����Ӫҵʱ���ڣ���ǰʱ��: {currentTime}");
        }

        // �����̼Ҷ����ܾ��쳣
        // merchantId: �̼�ID
        // orderId: ����ID
        // reason: �ܾ�ԭ��
        // ����: ҵ���쳣
        public static BusinessException MerchantOrderRejected(string merchantId, string orderId, string reason)
        {
            return new BusinessException(ErrorCodes.MerchantOrderRejected,
                $"�̼� (ID: {merchantId}) �ܾ����ܶ��� (ID: {orderId})��ԭ��: {reason}");
        }

        // ��ȡ�̼�״̬�ı�����
        // status: ״̬��
        // ����: ״̬�ı�����
        private static string GetMerchantStatusText(int status)
        {
            return status switch
            {
                1 => "����Ӫҵ",
                2 => "��ʱ��Ϣ",
                3 => "�ѹر�",
                _ => $"δ֪״̬({status})"
            };
        }
    }
}
