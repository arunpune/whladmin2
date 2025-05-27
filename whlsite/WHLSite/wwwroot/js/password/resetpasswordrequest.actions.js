$(function() {
  $('.whl-action-reset-password-request').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-reset-password-request').attr('disabled', 'disabled');
    $('.whl-action-reset-password-request').html('Requesting...');
    $('.whl-form-reset-password-request').submit();
  });
});