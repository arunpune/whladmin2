$(function () {
  $('.whl-action-reset-password-request').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-reset-password-request').attr('disabled', 'disabled');
    $('.whl-action-reset-password-request').html('Requesting...');
    var re = $('.whl-formfield-recaptchaenabled').val();
    console.log(re);
    if (re.toLowerCase() === '1') {
      grecaptcha.execute();
    } else {
      $('.whl-form-reset-password-request').submit();
    }
  });
});
function fnResetPasswordRequest(t) {
  console.debug('recaptcha token: ' + t);
  $('.whl-formfield-grecaptchatoken').val(t);
  $('.whl-form-reset-password-request').submit();
}