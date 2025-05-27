$(function() {
  $('.whl-action-resend-activation').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-resend-activation').attr('disabled', 'disabled');
    $('.whl-action-resend-activation').html('Requesting...');
    $('.whl-form-resend-activation').submit();
  });
});