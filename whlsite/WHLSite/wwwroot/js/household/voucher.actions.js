$(function() {

  $('.whl-action-save-voucherinfo').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-voucherinfo').hide();
    $('.whl-action-save-voucherinfo').attr('disabled', 'disabled');
    $('.whl-action-save-voucherinfo').html('Saving...');
    $('.whl-form-edit-voucherinfo').submit();
  });

});