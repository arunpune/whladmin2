$(function () {
  $('.whl-action-change-password').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-change-password').attr('disabled', 'disabled');
    $('.whl-action-change-password').html('Signing in...');
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
            $('.whl-form-change-password').submit();
          });
      });
    } else {
      $('.whl-form-change-password').submit();
    }
  });
});
function fnChangePassword(t) {
  console.debug('recaptcha token: ' + t);
  $('.whl-formfield-grecaptchatoken').val(t);
  $('.whl-form-change-password').submit();
}