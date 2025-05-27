$(function() {
  $('.whl-action-reset-password').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-reset-password').attr('disabled', 'disabled');
    $('.whl-action-reset-password').html('Requesting...');
    $('.whl-form-reset-password').submit();
  });
});