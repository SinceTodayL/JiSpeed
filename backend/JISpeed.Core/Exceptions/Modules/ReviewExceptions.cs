using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // ����ģ���쳣��
    public class ReviewExceptions
    {
        // ��������δ�ҵ��쳣
        // reviewId: ����ID
        // ����: δ�ҵ��쳣
        public static NotFoundException RatingNotFound(string reviewId)
        {
            return new NotFoundException(ErrorCodes.RatingNotFound, $"���� (ID: {reviewId}) δ�ҵ�");
        }

        // �����ظ������쳣
        // orderId: ����ID
        // userId: �û�ID
        // ����: ҵ���쳣
        public static BusinessException DuplicateRating(string orderId, string userId)
        {
            return new BusinessException(ErrorCodes.DuplicateRating,
                $"�û� (ID: {userId}) �ѶԶ��� (ID: {orderId}) ���й����ۣ������ظ�����");
        }

        // ��������Ȩ�޴����쳣
        // userId: �û�ID
        // orderId: ����ID
        // ����: ҵ���쳣
        public static BusinessException RatingPermissionError(string userId, string orderId)
        {
            return new BusinessException(ErrorCodes.RatingPermissionError,
                $"�û� (ID: {userId}) ��Ȩ�Զ��� (ID: {orderId}) ��������");
        }

        // ������������Υ���쳣
        // content: ��������
        // reason: Υ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException RatingContentViolation(string content, string reason)
        {
            return new BusinessException(ErrorCodes.RatingContentViolation,
                $"��������Υ�棬ԭ��: {reason}");
        }

        // ��������ʱ���ѹ��쳣
        // orderId: ����ID
        // deadline: ���۽�ֹʱ��
        // ����: ҵ���쳣
        public static BusinessException RatingTimeExpired(string orderId, DateTime deadline)
        {
            return new BusinessException(ErrorCodes.RatingTimeExpired,
                $"���� (ID: {orderId}) ������ʱ���ѹ�����ֹʱ��: {deadline.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
    }
}
