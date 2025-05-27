$(function () {
  $('.whl-action-reset-password').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-reset-password').attr('disabled', 'disabled');
    $('.whl-action-reset-password').html('Requesting...');
    var re = $('.whl-formfield-recaptchaenabled').val();
    console.log(re);
    if (re.toLowerCase() === '1') {
      grecaptcha.execute();
    } else {
      $('.whl-form-reset-password').submit();
    }
  });
});
function fnResetPassword(t) {
  console.debug('recaptcha token: ' + t);
  $('.whl-formfield-grecaptchatoken').val(t);
  $('.whl-form-reset-password').submit();
}