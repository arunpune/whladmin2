$(function () {

  $('.whl-action-faqtextsearch').attr('disabled', 'disabled');

  $('.whl-formfield-faqtextsearch').on('keypress', function (e) {
    var reg = /^[A-Za-z ]+$/;
    var k = e.key;
    return reg.test(e.key);
  });

  $('.whl-formfield-faqtextsearch').on('change', function (e) {
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $('.whl-action-faqtextsearch').removeAttr('disabled');
    } else {
      $('.whl-action-faqtextsearch').attr('disabled', 'disabled');
    }
  });

  $('.whl-action-faqtextsearch').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var v = $('.whl-formfield-faqtextsearch').val();
    $('.whl-faq-accordion').show();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      var matchedElements = $('.whl-faq-accordion-collapse').filter(function () {
        return $(this).text().toLowerCase().indexOf(v.toLowerCase()) > -1;
      });
      $('.whl-faq-accordion').hide();
      if (matchedElements.length > 0) {
        $(matchedElements).each((i, a) => {
          var id = $(a).data('faqid');
          $('.whl-faq-accordion-' + id).show();
        });
      }
    }
  });

  $('.whl-action-faqtextsearchreset').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-formfield-faqtextsearch').val('');
    $('.whl-action-faqtextsearch').attr('disabled', 'disabled');
    $('.whl-faq-accordion').show();
  });

});