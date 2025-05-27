$(function() {

  $('.whl-action-save-addressinfo').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-addressinfo').hide();
    $('.whl-action-save-addressinfo').attr('disabled', 'disabled');
    $('.whl-action-save-addressinfo').html('Saving...');
    $('.whl-form-edit-addressinfo').submit();
  });

});