$(function() {
  $('.whl-action-register').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-register').attr('disabled', 'disabled');
    $('.whl-action-register').html('Registering...');
    $('.whl-form-register').submit();
  });
});