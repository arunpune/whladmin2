$(function () {
  $('.whl-action-change-password').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-change-password').attr('disabled', 'disabled');
    $('.whl-action-change-password').html('Requesting...');
    var re = $('.whl-formfield-recaptchaenabled').val();
    console.log(re);
    if (re.toLowerCase() === '1') {
      grecaptcha.execute();
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