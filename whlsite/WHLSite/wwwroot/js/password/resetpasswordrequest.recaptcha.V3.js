$(function () {
  $('.whl-action-reset-password-request').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-reset-password-request').attr('disabled', 'disabled');
    $('.whl-action-reset-password-request').html('Signing in...');
    var re = $('.whl-formfield-recaptchaenabled').val();
    console.log(re);
    if (re.toLowerCase() === '1') {
      grecaptcha.enterprise.ready(async () => {
        const k = $('.whl-formfield-recaptchakey').val();
        const a = $('.whl-formfield-recaptchaaction').val();
        grecaptcha.enterprise.execute(k, { action: a })
          .then(function (t) {
            console.debug('recaptcha token: ' + t);
            $('.whl-formfield-grecaptchatoken').val(t);
            $('.whl-form-reset-password-request').submit();
          });
      });
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