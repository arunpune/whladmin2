$(function () {
  $('.whl-action-reset-password').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-reset-password').attr('disabled', 'disabled');
    $('.whl-action-reset-password').html('Signing in...');
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
            $('.whl-form-reset-password').submit();
          });
      });
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