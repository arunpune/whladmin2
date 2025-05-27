$(function() {

  $('.whl-action-save-liveinaideinfo').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-liveinaideinfo').hide();
    $('.whl-action-save-liveinaideinfo').attr('disabled', 'disabled');
    $('.whl-action-save-liveinaideinfo').html('Saving...');
    $('.whl-form-edit-liveinaideinfo').submit();
  });

});