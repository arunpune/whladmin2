$(function () {
  $('.whl-action-login').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-login').attr('disabled', 'disabled');
    $('.whl-action-login').html('Signing in...');
    var re = $('.whl-formfield-recaptchaenabled').val();
    console.log(re);
    if (re.toLowerCase() === '1') {
      grecaptcha.execute();
    } else {
      $('.whl-form-login').submit();
    }
  });
});
function fnLogin(t) {
  console.debug('recaptcha token: ' + t);
  $('.whl-formfield-grecaptchatoken').val(t);
  $('.whl-form-login').submit();
}