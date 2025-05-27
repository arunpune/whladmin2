$(function() {
  $('.whl-action-change-password').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-change-password').attr('disabled', 'disabled');
    $('.whl-action-change-password').html('Requesting...');
    $('.whl-form-change-password').submit();
  });
});