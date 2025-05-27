$(function() {
  $('.whl-action-login').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-login').attr('disabled', 'disabled');
    $('.whl-action-login').html('Signing in...');
    $('.whl-form-login').submit();
  });
});