$(function() {

  $('.whl-action-save-contactinfo').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-contactinfo').hide();
    $('.whl-action-save-contactinfo').attr('disabled', 'disabled');
    $('.whl-action-save-contactinfo').html('Saving...');
    $('.whl-form-edit-contactinfo').submit();
  });

});