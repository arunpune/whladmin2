$(function () {

  // Disable submission
  $('.whl-action-save-liveinaideinfo').attr('disabled', 'disabled');
  fnToggleSaveLiveInAideInfo();

  $('.whl-formfield-liveinaideind').on('change', function (e) {
    fnToggleSaveLiveInAideInfo();
  });

  function fnToggleSaveLiveInAideInfo() {
    var li = $('.whl-formfield-liveinaideind').prop('checked');
    var allowSubmission = true;
    if (allowSubmission) {
      $('.whl-action-save-liveinaideinfo').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-liveinaideinfo').attr('disabled', 'disabled');
    }
  }

});